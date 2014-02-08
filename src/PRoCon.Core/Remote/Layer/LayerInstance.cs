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

        public delegate void LayerAccountHandler(ILayerClient client);
        public event LayerAccountHandler ClientConnected;
        //public event LayerAccountHandler LayerAccountLogout;

        private TcpListener _layerListener;

        private PRoConApplication _application;
        private PRoConClient _client;

        public AccountPrivilegeDictionary AccountPrivileges {
            get;
            private set;
        }

        public Dictionary<String, ILayerClient> LayerClients {
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
            this.LayerClients = new Dictionary<String, ILayerClient>();
            
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

            this.ClientConnected += PRoConLayer_ClientConnected;

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

        private void PRoConLayer_ClientConnected(ILayerClient sender) {
            sender.Login += client_LayerClientLogin;
            sender.Logout += client_LayerClientLogout;
            sender.Quit += client_LayerClientQuit;
            sender.ClientShutdown += client_LayerClientShutdown;
            sender.UidRegistered += client_UidRegistered;
        }

        private void client_LayerClientShutdown(ILayerClient sender) {
            sender.Login -= client_LayerClientLogin;
            sender.Logout -= client_LayerClientLogout;
            sender.Quit -= client_LayerClientQuit;
            sender.ClientShutdown -= client_LayerClientShutdown;
            sender.UidRegistered -= client_UidRegistered;

            this.LayerClients.Remove(sender.IPPort);

            this.BroadcastAccountLogout(sender.Username);
        }

        private void BroadcastAccountLogout(string username) {
            foreach (ILayerClient client in new List<ILayerClient>(this.LayerClients.Values)) {
                client.SendAccountLogout(username);
            }
        }

        private void client_LayerClientQuit(ILayerClient sender) {
            if (this.LayerClients.ContainsKey(sender.IPPort) == true) {
                this.LayerClients.Remove(sender.IPPort);
                this.BroadcastAccountLogout(sender.Username);
            }
        }

        private void client_LayerClientLogout(ILayerClient sender) {
            this.BroadcastAccountLogout(sender.Username);
        }

        private void client_LayerClientLogin(ILayerClient sender) {
            foreach (ILayerClient client in new List<ILayerClient>(this.LayerClients.Values)) {
                client.SendAccountLogin(sender.Username, sender.Privileges);
            }
        }

        private void client_UidRegistered(ILayerClient sender) {
            foreach (ILayerClient client in new List<ILayerClient>(this.LayerClients.Values)) {
                client.SendRegisteredUid(sender.ProconEventsUid, sender.Username);
            }
        }

        private void ForcefullyDisconnectAccount(string accountName) {
            List<ILayerClient> shutDownClients = new List<ILayerClient>();

            foreach (ILayerClient client in new List<ILayerClient>(this.LayerClients.Values)) {
                if (String.CompareOrdinal(client.Username, accountName) == 0) {
                    shutDownClients.Add(client);
                }
            }

            foreach (ILayerClient cplcShutdown in shutDownClients) {
                cplcShutdown.Shutdown();
            }

            this.BroadcastAccountLogout(accountName);
        }

        public List<String> GetLoggedInAccountUsernames() {
            List<String> loggedInAccountUsernames = new List<String>();

            foreach (ILayerClient client in new List<ILayerClient>(this.LayerClients.Values)) {
                if (loggedInAccountUsernames.Contains(client.Username) == false) {
                    loggedInAccountUsernames.Add(client.Username);
                }
            }

            return loggedInAccountUsernames;
        }

        public List<String> GetLoggedInAccountUsernamesWithUids(bool listUids) {
            List<String> loggedInAccounts = new List<String>();

            foreach (ILayerClient plcConnection in new List<ILayerClient>(this.LayerClients.Values)) {
                if (loggedInAccounts.Contains(plcConnection.Username) == false) {
                    loggedInAccounts.Add(plcConnection.Username);

                    if (listUids == true) {
                        loggedInAccounts.Add(plcConnection.ProconEventsUid);
                    }
                }
            }

            return loggedInAccounts;
        }

        private void ListenIncommingLayerConnections(IAsyncResult ar) {

            if (this._layerListener != null) {

                try {
                    TcpClient tcpNewConnection = this._layerListener.EndAcceptTcpClient(ar);

                    ILayerClient cplcNewConnection = new LayerClient(new LayerConnection(tcpNewConnection), this._application, this._client);

                    // Issue #24. Somewhere the end port connection+port isn't being removed.
                    if (this.LayerClients.ContainsKey(cplcNewConnection.IPPort) == true) {
                        this.LayerClients[cplcNewConnection.IPPort].Shutdown();

                        // If, for some reason, the client wasn't removed during shutdown..
                        if (this.LayerClients.ContainsKey(cplcNewConnection.IPPort) == true) {
                            this.LayerClients.Remove(cplcNewConnection.IPPort);
                        }
                    }

                    this.LayerClients.Add(cplcNewConnection.IPPort, cplcNewConnection);

                    if (this.ClientConnected != null) {
                        FrostbiteConnection.RaiseEvent(this.ClientConnected.GetInvocationList(), cplcNewConnection);
                    }

                    this._layerListener.BeginAcceptTcpClient(this.ListenIncommingLayerConnections, this);
                }
                catch (SocketException exception) {

                    if (this.LayerSocketError != null) {
                        FrostbiteConnection.RaiseEvent(this.LayerSocketError.GetInvocationList(), exception);
                    }

                    this.ShutdownLayerListener();

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
            foreach (ILayerClient client in new List<ILayerClient>(this.LayerClients.Values)) {
                client.Poke();
            }
        }

        private IPAddress ResolveHostName(string hostName) {
            IPAddress ipReturn = IPAddress.None;

            if (IPAddress.TryParse(hostName, out ipReturn) == false) {

                ipReturn = IPAddress.None;

                try {
                    IPHostEntry iphHost = Dns.GetHostEntry(hostName);

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

                this._layerListener.BeginAcceptTcpClient(this.ListenIncommingLayerConnections, this);
            }
            catch (SocketException skeError) {
                if (this.LayerSocketError != null) {
                    FrostbiteConnection.RaiseEvent(this.LayerSocketError.GetInvocationList(), skeError);
                }

                this.ShutdownLayerListener();
            }
        }

        public void ShutdownLayerListener() {

            if (this._layerListener != null) {

                try {
                    foreach (ILayerClient client in new List<ILayerClient>(this.LayerClients.Values)) {
                        client.Shutdown();
                    }

                    this._layerListener.Stop();
                    this._layerListener = null;
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
