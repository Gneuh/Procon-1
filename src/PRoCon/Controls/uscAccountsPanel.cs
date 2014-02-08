using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Net.Sockets;

namespace PRoCon {
    using Core;
    using Core.Accounts;
    using Core.Remote.Layer;
    using Core.Remote;
    using PRoCon.Forms;

    public partial class uscAccountsPanel : UserControl {

        private frmMain m_frmMain;
        private uscServerConnection m_uscConnectionPanel;
        private CLocalization m_clocLanguage;

        private PRoConApplication m_praApplication;
        private PRoConClient m_prcClient;

        //private CPrivileges m_spPrivileges;

        // This variable is only used by this panel to show an example of the Layer Name.
        private string m_strServerName;
        public string ServerName {
            set { 
                this.m_strServerName = value;
                this.lblExampleLayerName.Text = this.txtLayerName.Text.Replace("%servername%", this.m_strServerName);
            }
        }

        public uscAccountsPanel() {
            InitializeComponent();

            //this.m_uscParentPanel = null;
            this.m_frmMain = null;
            this.m_clocLanguage = null;

            //this.m_spPrivileges = new CPrivileges();
            //this.m_spPrivileges.PrivilegesFlags = CPrivileges.FullPrivilegesFlags;

            this.uscPrivileges.OnUpdatePrivileges += new uscPrivilegesSelection.OnUpdatePrivilegesDelegate(uscPrivileges_OnUpdatePrivileges);
            this.uscPrivileges.OnCancelPrivileges += new uscPrivilegesSelection.OnCancelPrivilegesDelegate(uscPrivileges_OnCancelPrivileges);
        }

        private void uscAccountsPanel_Load(object sender, EventArgs e) {

            if (this.m_praApplication != null && this.m_prcClient != null) {
                foreach (Account accLoadedAccount in this.m_praApplication.AccountsList) {
                    this.AccountsList_AccountAdded(accLoadedAccount);

                    if (this.m_prcClient.Layer.AccountPrivileges.Contains(accLoadedAccount.Name) == true) {
                        this.uscAccountsPanel_AccountPrivilegesChanged(this.m_prcClient.Layer.AccountPrivileges[accLoadedAccount.Name]);
                    }
                }

                this.txtLayerName.Text = this.m_prcClient.Layer.NameFormat;
                this.txtLayerBindingAddress.Text = this.m_prcClient.Layer.BindingAddress;
                this.txtLayerStartPort.Text = this.m_prcClient.Layer.ListeningPort.ToString();

                if (this.m_prcClient.Layer.IsOnline == true) {
                    this.Layer_LayerOnline();
                }
            }
        }

        public void Initalize(frmMain frmMain, uscServerConnection uscConnectionPanel) {
            this.m_frmMain = frmMain;
            this.m_uscConnectionPanel = uscConnectionPanel;

            this.picLayerServerStatus.Image = this.m_frmMain.picLayerOffline.Image;

            this.pnlMainLayerServer.Dock = DockStyle.Fill;
            this.pnlStartPRoConLayer.Dock = DockStyle.Fill;
            this.pnlAccountPrivileges.Dock = DockStyle.Fill;

            this.lsvLayerAccounts.SmallImageList = this.m_frmMain.iglIcons;
        }

        public void SetConnection(PRoConApplication praApplication, PRoConClient prcClient) {
            if ((this.m_praApplication = praApplication) != null && (this.m_prcClient = prcClient) != null) {

                if (this.m_prcClient.Game != null) {
                    this.m_prcClient_GameTypeDiscovered(prcClient);
                }
                else {
                    this.m_prcClient.GameTypeDiscovered += new PRoConClient.EmptyParamterHandler(m_prcClient_GameTypeDiscovered);
                }
            }
        }

        void m_prcClient_GameTypeDiscovered(PRoConClient sender) {

            this.m_praApplication.AccountsList.AccountAdded += new PRoCon.Core.Accounts.AccountDictionary.AccountAlteredHandler(AccountsList_AccountAdded);
            this.m_praApplication.AccountsList.AccountRemoved += new PRoCon.Core.Accounts.AccountDictionary.AccountAlteredHandler(AccountsList_AccountRemoved);

            foreach (Account acAccount in this.m_praApplication.AccountsList) {
                acAccount.AccountPasswordChanged += new Account.AccountPasswordChangedHandler(acAccount_AccountPasswordChanged);

                if (this.m_prcClient.Layer.AccountPrivileges.Contains(acAccount.Name) == true) {
                    this.m_prcClient.Layer.AccountPrivileges[acAccount.Name].AccountPrivilegesChanged += new AccountPrivilege.AccountPrivilegesChangedHandler(uscAccountsPanel_AccountPrivilegesChanged);
                }
            }

            this.m_prcClient.Layer.LayerStarted += Layer_LayerOnline;
            this.m_prcClient.Layer.LayerShutdown += Layer_LayerOffline;
            this.m_prcClient.Layer.SocketError += Layer_LayerSocketError;

            this.m_prcClient.Layer.ClientConnected += Layer_ClientConnected;
        }

        void Layer_ClientConnected(ILayerClient client) {
            client.Login += Layer_LayerClientLogin;
            client.Logout += Layer_LayerClientLogout;
        }

        public void SetLocalization(CLocalization clocLanguage) {

            if ((this.m_clocLanguage = clocLanguage) != null) {

                this.lblLayerServerSetupTitle.Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.lblLayerServerSetupTitle", null);
                this.lnkStartStopLayer.Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.lnkStartStopLayer.Start", null);
                this.lblLayerServerStatus.Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.lblLayerServerStatus.Offline", null);

                this.lblLayerAssignAccountPrivilegesTitle.Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.lblLayerAssignAccountPrivilegesTitle", null);

                this.colAccounts.Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.lstLayerAccounts.colAccounts", null);
                this.colPrivileges.Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.lstLayerAccounts.colPrivileges", null);
                this.colRConAccess.Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.lstLayerAccounts.colRConAccess", null);

                this.lnkManageAccounts.Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.lnkManageAccounts", null);

                this.lblLayerStartTitle.Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.lblLayerStartTitle", null);
                this.lblLayerStartPort.Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.lblLayerStartPort", null);
                this.lblLayerBindingIP.Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.lblLayerBindingIP", null);
                this.lblBindingExplanation.Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.lblBindingExplanation", null);
                this.lblLayerName.Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.lblLayerName", null);
                this.btnInsertName.Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.btnInsertName", null);
                this.lblExampleLayerName.Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.lblExampleLayerName", new string[] { this.txtLayerName.Text.Replace("%servername%", this.m_strServerName) });
                this.btnLayerStart.Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.btnLayerStart", null);
                this.btnCancelLayerStart.Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.btnCancelLayerStart", null);

                this.uscPrivileges.SetLocalization(this.m_clocLanguage);

                this.RefreshLayerPrivilegesPanel();

            }
        }

        public delegate void ManageAccountsRequestDelegate(object sender, EventArgs e);
        public event ManageAccountsRequestDelegate ManageAccountsRequest;

        private bool m_blEditingPrivileges = false;
        private void ShowLayerPanel(Panel pnlShow) {
            this.pnlMainLayerServer.Hide();
            this.pnlStartPRoConLayer.Hide();
            this.pnlAccountPrivileges.Hide();

            this.m_blEditingPrivileges = false;

            if (pnlShow == this.pnlMainLayerServer) {
                this.lsvLayerAccounts.SelectedItems.Clear();
            }
            else if (pnlShow == this.pnlAccountPrivileges) {
                this.m_blEditingPrivileges = true;

                // Should be but still..
                if (this.lsvLayerAccounts.SelectedItems.Count > 0) {
                    this.uscPrivileges.AccountName = this.lsvLayerAccounts.SelectedItems[0].Text;
                }

                if (this.lsvLayerAccounts.SelectedItems.Count > 0) {
                    CPrivileges spPrivs = (CPrivileges)this.lsvLayerAccounts.SelectedItems[0].SubItems[1].Tag;

                    this.uscPrivileges.Privileges = spPrivs;
                }
                else {
                    this.uscPrivileges.Privileges = new CPrivileges();
                }
            }

            pnlShow.Show();
        }

        private void RefreshLayerPrivilegesPanel() {
            foreach (ListViewItem lviItem in this.lsvLayerAccounts.Items) {
                if (lviItem.SubItems[1].Tag != null && this.m_clocLanguage != null && this.m_prcClient != null) {

                    if (this.m_prcClient.Layer.AccountPrivileges.Contains(lviItem.Text) == true) {
                        CPrivileges spDetails = this.m_prcClient.Layer.AccountPrivileges[lviItem.Text].Privileges;
                    
                        if (spDetails.HasNoRconAccess == true) {
                            lviItem.SubItems["rconaccess"].Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.lstLayerAccounts.Privileges.None", null);
                        }
                        else if (spDetails.HasLimitedRconAccess == true) {
                            lviItem.SubItems["rconaccess"].Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.lstLayerAccounts.Privileges.Limited", null);
                        }
                        else {
                            lviItem.SubItems["rconaccess"].Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.lstLayerAccounts.Privileges.Full", null);
                        }

                        if (spDetails.HasNoLocalAccess == true) {
                            lviItem.SubItems["localaccess"].Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.lstLayerAccounts.Privileges.None", null);
                        }
                        else if (spDetails.HasLimitedLocalAccess == true) {
                            lviItem.SubItems["localaccess"].Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.lstLayerAccounts.Privileges.Limited", null);
                        }
                        else {
                            lviItem.SubItems["localaccess"].Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.lstLayerAccounts.Privileges.Full", null);
                        }
                    }
                }
            }

            if (this.m_blEditingPrivileges == true) {
                this.ShowLayerPanel(this.pnlAccountPrivileges);
            }

            foreach (ColumnHeader ch in this.lsvLayerAccounts.Columns) {
                ch.Width = -2;
            }
        }

        void uscAccountsPanel_AccountPrivilegesChanged(AccountPrivilege item) {

            if (this.lsvLayerAccounts.Items.ContainsKey(item.Owner.Name) == true) {
                ListViewItem lviAccount = this.lsvLayerAccounts.Items[item.Owner.Name];

                lviAccount.SubItems["rconaccess"].Tag = item.Privileges;

                this.RefreshLayerPrivilegesPanel();
            }
        }

        void acAccount_AccountPasswordChanged(Account item) {

            if (this.lsvLayerAccounts.Items.ContainsKey(item.Name) == true) {
                this.lsvLayerAccounts.Items[item.Name].Tag = item.Password;
            }
        }

        void AccountsList_AccountRemoved(Account item) {
            item.AccountPasswordChanged -= new Account.AccountPasswordChangedHandler(acAccount_AccountPasswordChanged);

            if (this.lsvLayerAccounts.Items.ContainsKey(item.Name) == true) {
                this.lsvLayerAccounts.Items.Remove(this.lsvLayerAccounts.Items[item.Name]);
            }
        }

        void AccountsList_AccountAdded(Account item) {
            item.AccountPasswordChanged += new Account.AccountPasswordChangedHandler(acAccount_AccountPasswordChanged);

            if (this.lsvLayerAccounts.Items.ContainsKey(item.Name) == false) {

                ListViewItem lviNewAccount = new ListViewItem(item.Name);
                lviNewAccount.Name = item.Name;
                lviNewAccount.Tag = item.Password;
                lviNewAccount.ImageKey = "status_offline.png";

                ListViewItem.ListViewSubItem lsviNewSubitem = new ListViewItem.ListViewSubItem();
                //lsviNewSubitem.Text = "none";
                lsviNewSubitem.Name = "rconaccess";
                lsviNewSubitem.Tag = new CPrivileges();
                lviNewAccount.SubItems.Add(lsviNewSubitem);

                lsviNewSubitem = new ListViewItem.ListViewSubItem();
                //lsviNewSubitem.Text = "none";
                lsviNewSubitem.Name = "localaccess";
                lviNewAccount.SubItems.Add(lsviNewSubitem);

                lsviNewSubitem = new ListViewItem.ListViewSubItem();
                lsviNewSubitem.Text = String.Empty;
                lsviNewSubitem.Name = "ip";
                lviNewAccount.SubItems.Add(lsviNewSubitem);

                this.lsvLayerAccounts.Items.Add(lviNewAccount);

                this.RefreshLayerPrivilegesPanel();
            }
        }
        
        private void lnkStartStopLayer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {

            if (this.m_prcClient.Layer.IsOnline == false) {
                this.ShowLayerPanel(this.pnlStartPRoConLayer);
            }
            else {
                this.m_prcClient.Layer.IsEnabled = false;

                this.m_prcClient.Layer.Shutdown();
            }
        }

        private void btnLayerStart_Click(object sender, EventArgs e) {
            this.ShowLayerPanel(this.pnlMainLayerServer);

            this.m_prcClient.Layer.IsEnabled = true;
            this.m_prcClient.Layer.BindingAddress = this.txtLayerBindingAddress.Text;
            this.m_prcClient.Layer.ListeningPort = Convert.ToUInt16(this.txtLayerStartPort.Text);
            this.m_prcClient.Layer.NameFormat = this.txtLayerName.Text;
            this.m_prcClient.Layer.Start();
        }

        private void btnCancelLayerStart_Click(object sender, EventArgs e) {
            this.ShowLayerPanel(this.pnlMainLayerServer);
        }

        private void lstLayerAccounts_SelectedIndexChanged(object sender, EventArgs e) {

            if (this.lsvLayerAccounts.SelectedItems.Count > 0) {

                this.ShowLayerPanel(this.pnlAccountPrivileges);
            }
        }

        void uscPrivileges_OnCancelPrivileges() {
            this.ShowLayerPanel(this.pnlMainLayerServer);
        }

        void uscPrivileges_OnUpdatePrivileges(string strAccountName, CPrivileges spUpdatedPrivs) {
            if (this.m_prcClient.Layer.AccountPrivileges.Contains(strAccountName) == true) {
                this.m_prcClient.Layer.AccountPrivileges[strAccountName].SetPrivileges(spUpdatedPrivs);
            }

            this.ShowLayerPanel(this.pnlMainLayerServer);

            this.RefreshLayerPrivilegesPanel();
        }

        private void lnkManageAccounts_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            this.ManageAccountsRequest(this, new EventArgs());
        }

        #region Layer Events and Helper Methods

        private void Layer_LayerOnline() {
            this.InvokeIfRequired(() => {
                this.picLayerServerStatus.Image = this.m_frmMain.picLayerOnline.Image;

                this.lblLayerServerStatus.Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.lblLayerServerStatus.Online", new[] {this.m_prcClient.Layer.ListeningPort.ToString(CultureInfo.InvariantCulture)});
                this.lblLayerServerStatus.ForeColor = Color.ForestGreen;

                this.lnkStartStopLayer.Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.lnkStartStopLayer.Stop", null);
            });
        }

        private void Layer_LayerOffline() {
            this.InvokeIfRequired(() => {
                this.picLayerServerStatus.Image = this.m_frmMain.picLayerOffline.Image;

                this.lblLayerServerStatus.Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.lblLayerServerStatus.Offline", null);
                this.lblLayerServerStatus.ForeColor = Color.Maroon;

                this.lnkStartStopLayer.Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.lnkStartStopLayer.Start", null);
            });
        }

        private void Layer_LayerSocketError(SocketException se) {
            this.InvokeIfRequired(() => {
                this.picLayerServerStatus.Image = this.m_frmMain.picLayerOffline.Image;

                this.lblLayerServerStatus.Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.lblLayerServerStatus.Error", new[] { se.Message });
                this.lblLayerServerStatus.ForeColor = Color.Maroon;

                this.lnkStartStopLayer.Text = this.m_clocLanguage.GetLocalized("uscAccountsPanel.lnkStartStopLayer.Start", null);
            });
        }

        private void Layer_LayerClientLogin(ILayerClient sender) {
            this.InvokeIfRequired(() => {
                if (this.lsvLayerAccounts.Items.ContainsKey(sender.Username) == true) {
                    this.lsvLayerAccounts.Items[sender.Username].ImageKey = @"status_online.png";
                }
            });
        }

        private void Layer_LayerClientLogout(ILayerClient sender) {
            this.InvokeIfRequired(() => {
                if (this.lsvLayerAccounts.Items.ContainsKey(sender.Username) == true) {
                    this.lsvLayerAccounts.Items[sender.Username].ImageKey = @"status_offline.png";
                }
            });
        }

        #endregion

        private void txtLayerName_TextChanged(object sender, EventArgs e) {
            this.lblExampleLayerName.Text = this.txtLayerName.Text.Replace("%servername%", this.m_strServerName);
        }

        private void btnInsertName_Click(object sender, EventArgs e) {

            int iInsertPosition = this.txtLayerName.SelectionStart;

            this.txtLayerName.Text = this.txtLayerName.Text.Remove(iInsertPosition, this.txtLayerName.SelectionLength);
            this.txtLayerName.Text = this.txtLayerName.Text.Insert(iInsertPosition, "%servername%");
        }

        private void txtLayerStartPort_KeyPress(object sender, KeyPressEventArgs e) {
            e.Handled = (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b');
        }

    }
}
