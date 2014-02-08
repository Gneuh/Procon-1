using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace PRoCon.Core.Remote.Layer {
    using Core;
    using Core.Accounts;
    using Core.Remote;

    public class LayerInstance {

        public delegate void LayerEmptyParameterHandler();
        public event LayerEmptyParameterHandler LayerOnline;
        public event LayerEmptyParameterHandler LayerOffline;

        public delegate void LayerSocketErrorHandler(SocketException se);
        public event LayerSocketErrorHandler LayerSocketError;

        public delegate void LayerAccountHandler(LayerClient client);
        public event LayerAccountHandler ClientConnected;
        //public event LayerAccountHandler LayerAccountLogout;

        private TcpListener _layerListener;

        private PRoConApplication _application;
        private PRoConClient _client;

        public AccountPrivilegeDictionary AccountPrivileges {
            get;
            private set;
        }

        public Dictionary<String, LayerClient> LayerClients {
            get;
            private set;
        }

        public string BindingAddress {
            get;
            set;
        }

        public UInt16 ListeningPort {
            get;
            set;
        }

        public string LayerNameFormat {
            get;
            set;
        }

        public bool LayerEnabled {
            get;
            set;
        }

        public bool IsLayerOnline {
            get {
                return (this._layerListener != null);
            }
        }

        public LayerInstance() {
            this.AccountPrivileges = new AccountPrivilegeDictionary();

            this.ListeningPort = 27260;
            this.BindingAddress = "0.0.0.0";
            this.LayerNameFormat = "PRoCon[%servername%]";
            this._layerListener = null;
            this.LayerClients = new Dictionary<String, LayerClient>();
            
            this.LayerEnabled = false;

        }

        public void Initialize(PRoConApplication praApplication, PRoConClient prcClient) {
            this._application = praApplication;
            foreach (Account accAccount in this._application.AccountsList) {

                if (this.AccountPrivileges.Contains(accAccount.Name) == false) {
                    AccountPrivilege apPrivs = new AccountPrivilege(accAccount, new CPrivileges());
                    apPrivs.AccountPrivilegesChanged += new AccountPrivilege.AccountPrivilegesChangedHandler(apPrivs_AccountPrivilegesChanged);
                    this.AccountPrivileges.Add(apPrivs);
                }
                else {
                    this.AccountPrivileges[accAccount.Name].AccountPrivilegesChanged += new AccountPrivilege.AccountPrivilegesChangedHandler(apPrivs_AccountPrivilegesChanged);
                }

            }
            this._application.AccountsList.AccountAdded += new AccountDictionary.AccountAlteredHandler(AccountsList_AccountAdded);
            this._application.AccountsList.AccountRemoved += new AccountDictionary.AccountAlteredHandler(AccountsList_AccountRemoved);

            this._client = prcClient;

            this._client.SocketException += new PRoConClient.SocketExceptionHandler(m_prcClient_SocketException);
            this._client.ConnectionFailure += new PRoConClient.FailureHandler(m_prcClient_ConnectionFailure);
            this._client.ConnectionClosed += new PRoConClient.EmptyParamterHandler(m_prcClient_ConnectionClosed);
            this._client.Game.Login += new FrostbiteClient.EmptyParamterHandler(m_prcClient_CommandLogin);

            this.ClientConnected += new LayerAccountHandler(PRoConLayer_ClientConnected);

            if (this.LayerEnabled == true && this.IsLayerOnline == false) {
                this.StartLayerListener();
            }
        }

        private void AccountsList_AccountRemoved(Account item) {
            item.AccountPasswordChanged -= new Account.AccountPasswordChangedHandler(item_AccountPasswordChanged);
            this.AccountPrivileges[item.Name].AccountPrivilegesChanged -= new AccountPrivilege.AccountPrivilegesChangedHandler(apPrivs_AccountPrivilegesChanged);
            
            this.AccountPrivileges.Remove(item.Name);

            this.ForcefullyDisconnectAccount(item.Name);
        }

        private void AccountsList_AccountAdded(Account item) {
            AccountPrivilege apPrivs = new AccountPrivilege(item, new CPrivileges());

            this.AccountPrivileges.Add(apPrivs);

            item.AccountPasswordChanged += new Account.AccountPasswordChangedHandler(item_AccountPasswordChanged);
            apPrivs.AccountPrivilegesChanged += new AccountPrivilege.AccountPrivilegesChangedHandler(apPrivs_AccountPrivilegesChanged);
        }

        private void item_AccountPasswordChanged(Account item) {
            this.ForcefullyDisconnectAccount(item.Name);
        }

        private void apPrivs_AccountPrivilegesChanged(AccountPrivilege item) {
            if (item.Privileges.CanLogin == false) {
                this.ForcefullyDisconnectAccount(item.Owner.Name);
            }
        }

        private void m_prcClient_CommandLogin(FrostbiteClient sender) {
            // Start the layer if it's been enabled to on startup.
            if (this.LayerEnabled == true && this.IsLayerOnline == false) {
                this.StartLayerListener();
            }
        }

        private void m_prcClient_ConnectionClosed(PRoConClient sender) {
            this.ShutdownLayerListener();
        }

        private void m_prcClient_ConnectionFailure(PRoConClient sender, Exception exception) {
            this.ShutdownLayerListener();
        }

        private void m_prcClient_SocketException(PRoConClient sender, SocketException se) {
            this.ShutdownLayerListener();
        }

        private void PRoConLayer_ClientConnected(LayerClient client) {
            client.Login += new LayerClient.LayerClientHandler(client_LayerClientLogin);
            client.Logout += new LayerClient.LayerClientHandler(client_LayerClientLogout);
            client.Quit += new LayerClient.LayerClientHandler(client_LayerClientQuit);
            client.ClientShutdown += new LayerClient.LayerClientHandler(client_LayerClientShutdown);
            client.UidRegistered += new LayerClient.LayerClientHandler(client_UidRegistered);
        }
        
        private void client_LayerClientShutdown(LayerClient sender) {
            sender.Login -= new LayerClient.LayerClientHandler(client_LayerClientLogin);
            sender.Logout -= new LayerClient.LayerClientHandler(client_LayerClientLogout);
            sender.Quit -= new LayerClient.LayerClientHandler(client_LayerClientQuit);
            sender.ClientShutdown -= new LayerClient.LayerClientHandler(client_LayerClientShutdown);
            sender.UidRegistered -= new LayerClient.LayerClientHandler(client_UidRegistered);

            this.LayerClients.Remove(sender.IPPort);

            this.SendAccountLogout(sender.Username);
        }

        private void SendAccountLogout(string username) {
            foreach (LayerClient client in new List<LayerClient>(this.LayerClients.Values)) {
                client.OnAccountLogout(username);
            }
        }

        private void client_LayerClientQuit(LayerClient sender) {
            if (this.LayerClients.ContainsKey(sender.IPPort) == true) {
                this.LayerClients.Remove(sender.IPPort);
                this.SendAccountLogout(sender.Username);
            }
        }

        private void client_LayerClientLogout(LayerClient sender) {
            this.SendAccountLogout(sender.Username);
        }

        private void client_LayerClientLogin(LayerClient sender) {
            if (this.LayerClients.ContainsKey(sender.Username) == true) {
                // List a logged in account
            }

            foreach (LayerClient clcClient in new List<LayerClient>(this.LayerClients.Values)) {
                clcClient.OnAccountLogin(sender.Username, sender.Privileges);
            }
        }

        private void client_UidRegistered(LayerClient sender) {
            foreach (LayerClient clcClient in new List<LayerClient>(this.LayerClients.Values)) {
                clcClient.OnRegisteredUid(sender.ProconEventsUid, sender.Username);
            }
        }

        private void ForcefullyDisconnectAccount(string strAccountName) {
            List<LayerClient> lstShutDownClients = new List<LayerClient>();

            foreach (LayerClient plcConnection in new List<LayerClient>(this.LayerClients.Values)) {
                if (String.CompareOrdinal(plcConnection.Username, strAccountName) == 0) {
                    lstShutDownClients.Add(plcConnection);
                }
            }

            foreach (LayerClient cplcShutdown in lstShutDownClients) {
                cplcShutdown.Shutdown();
            }

            this.SendAccountLogout(strAccountName);
        }

        public List<string> GetLoggedInAccounts() {
            List<string> lstLoggedInAccounts = new List<string>();

            foreach (LayerClient plcConnection in new List<LayerClient>(this.LayerClients.Values)) {
                if (lstLoggedInAccounts.Contains(plcConnection.Username) == false) {
                    lstLoggedInAccounts.Add(plcConnection.Username);
                }
            }

            return lstLoggedInAccounts;
        }

        public List<string> GetLoggedInAccounts(bool listUids) {
            List<string> lstLoggedInAccounts = new List<string>();

            foreach (LayerClient plcConnection in new List<LayerClient>(this.LayerClients.Values)) {
                if (lstLoggedInAccounts.Contains(plcConnection.Username) == false) {
                    lstLoggedInAccounts.Add(plcConnection.Username);

                    if (listUids == true) {
                        lstLoggedInAccounts.Add(plcConnection.ProconEventsUid);
                    }
                }
            }

            return lstLoggedInAccounts;
        }

        /*
        private List<string> LayerGetAccounts() {

            List<string> lstReturnWords = new List<string>();

            foreach (AccountPrivilege apAccount in this.AccountPrivileges) {
                lstReturnWords.Add(apAccount.Owner.Name);
                lstReturnWords.Add(Convert.ToString(apAccount.Privileges.PrivilegesFlags));
            }

            return lstReturnWords;
        }
        */

        private AsyncCallback m_asyncAcceptCallback = new AsyncCallback(LayerInstance.ListenIncommingLayerConnections);
        private static void ListenIncommingLayerConnections(IAsyncResult ar) {

            LayerInstance plLayer = (LayerInstance)ar.AsyncState;

            if (plLayer._layerListener != null) {

                try {
                    TcpClient tcpNewConnection = plLayer._layerListener.EndAcceptTcpClient(ar);

                    LayerClient cplcNewConnection = new LayerClient(new LayerConnection(tcpNewConnection), plLayer._application, plLayer._client);

                    // Issue #24. Somewhere the end port connection+port isn't being removed.
                    if (plLayer.LayerClients.ContainsKey(cplcNewConnection.IPPort) == true) {
                        plLayer.LayerClients[cplcNewConnection.IPPort].Shutdown();

                        // If, for some reason, the client wasn't removed during shutdown..
                        if (plLayer.LayerClients.ContainsKey(cplcNewConnection.IPPort) == true) {
                            plLayer.LayerClients.Remove(cplcNewConnection.IPPort);
                        }
                    }

                    plLayer.LayerClients.Add(cplcNewConnection.IPPort, cplcNewConnection);

                    if (plLayer.ClientConnected != null) {
                        FrostbiteConnection.RaiseEvent(plLayer.ClientConnected.GetInvocationList(), cplcNewConnection);
                    }

                    plLayer._layerListener.BeginAcceptTcpClient(plLayer.m_asyncAcceptCallback, plLayer);
                }
                catch (SocketException exception) {

                    if (plLayer.LayerSocketError != null) {
                        FrostbiteConnection.RaiseEvent(plLayer.LayerSocketError.GetInvocationList(), exception);
                    }

                    plLayer.ShutdownLayerListener();

                    //cbfAccountsPanel.OnLayerServerSocketError(skeError);
                }
                catch (Exception e) {
                    FrostbiteConnection.LogError("ListenIncommingLayerConnections", "catch (Exception e)", e);
                }
            }
        }

        /// <summary>
        /// Pokes all connections, making sure they are still alive and well. Shuts them down if no traffic has occured in
        /// the last five minutes.
        /// </summary>
        public void Poke() {

            foreach (LayerClient client in new List<LayerClient>(this.LayerClients.Values)) {
                client.Game.Connection.Poke();
            }
        }

        private IPAddress ResolveHostName(string strHostName) {
            IPAddress ipReturn = IPAddress.None;

            if (IPAddress.TryParse(strHostName, out ipReturn) == false) {

                ipReturn = IPAddress.None;

                try {
                    IPHostEntry iphHost = Dns.GetHostEntry(strHostName);

                    if (iphHost.AddressList.Length > 0) {
                        ipReturn = iphHost.AddressList[0];
                    }
                    // ELSE return IPAddress.None..
                }
                catch (Exception) { } // Returns IPAddress.None..
            }

            return ipReturn;
        }

        public void StartLayerListener() {

            try {
                IPAddress ipBinding = this.ResolveHostName(this.BindingAddress);

                this._layerListener = new TcpListener(ipBinding, this.ListeningPort);

                this._layerListener.Start();

                if (this.LayerOnline != null) {
                    FrostbiteConnection.RaiseEvent(this.LayerOnline.GetInvocationList());
                }

                //this.OnLayerServerOnline();

                this._layerListener.BeginAcceptTcpClient(this.m_asyncAcceptCallback, this);
            }
            catch (SocketException skeError) {

                if (this.LayerSocketError != null) {
                    FrostbiteConnection.RaiseEvent(this.LayerSocketError.GetInvocationList(), skeError);
                }

                this.ShutdownLayerListener();
                //this.OnLayerServerSocketError(skeError);
            }
        }

        public void ShutdownLayerListener() {

            if (this._layerListener != null) {

                try {
                    foreach (LayerClient client in new List<LayerClient>(this.LayerClients.Values)) {
                        client.OnShutdown();
                        client.Shutdown();
                    }

                    //if (this.m_tclLayerListener != null) {
                        this._layerListener.Stop();
                        this._layerListener = null;
                    //}
                }
                catch (Exception) { }

                if (this.LayerOffline != null) {
                    FrostbiteConnection.RaiseEvent(this.LayerOffline.GetInvocationList());
                }
            }
            //this.OnLayerServerOffline();
        }
        
    }
         
}
