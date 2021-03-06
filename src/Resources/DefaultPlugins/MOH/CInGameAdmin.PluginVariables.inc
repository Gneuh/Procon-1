       // Lists only variables you want shown.. for instance enabling one option might hide another option
        // It's the best I got until I implement a way for plugins to display their own small interfaces.
        public List<CPluginVariable> GetDisplayPluginVariables() {

            List<CPluginVariable> lstReturn = new List<CPluginVariable>();

            lstReturn.Add(new CPluginVariable("Response Scope|Private Prefix", this.m_strPrivatePrefix.GetType(), this.m_strPrivatePrefix));
            lstReturn.Add(new CPluginVariable("Response Scope|Admins Prefix", this.m_strAdminsPrefix.GetType(), this.m_strAdminsPrefix));
            lstReturn.Add(new CPluginVariable("Response Scope|Public Prefix", this.m_strPublicPrefix.GetType(), this.m_strPublicPrefix));

            lstReturn.Add(new CPluginVariable("Commands|Kick", this.m_strKickCommand.GetType(), this.m_strKickCommand));

            lstReturn.Add(new CPluginVariable("Commands|Move", this.m_strMoveCommand.GetType(), this.m_strMoveCommand));
            lstReturn.Add(new CPluginVariable("Commands|Force Move", this.m_strForceMoveCommand.GetType(), this.m_strForceMoveCommand));
            lstReturn.Add(new CPluginVariable("Commands|Nuke", this.m_strNukeCommand.GetType(), this.m_strNukeCommand));
            lstReturn.Add(new CPluginVariable("Commands|Kill", this.m_strKillCommand.GetType(), this.m_strKillCommand));
            lstReturn.Add(new CPluginVariable("Commands|Temporary Ban", this.m_strTemporaryBanCommand.GetType(), this.m_strTemporaryBanCommand));
            lstReturn.Add(new CPluginVariable("Commands|Permanent Ban", this.m_strPermanentBanCommand.GetType(), this.m_strPermanentBanCommand));
            lstReturn.Add(new CPluginVariable("Commands|Say", this.m_strSayCommand.GetType(), this.m_strSayCommand));
            lstReturn.Add(new CPluginVariable("Commands|Player Say", this.m_strPlayerSayCommand.GetType(), this.m_strPlayerSayCommand));

            lstReturn.Add(new CPluginVariable("Commands|Restart Map", this.m_strRestartLevelCommand.GetType(), this.m_strRestartLevelCommand));
            lstReturn.Add(new CPluginVariable("Commands|Next Map", this.m_strNextLevelCommand.GetType(), this.m_strNextLevelCommand));
            lstReturn.Add(new CPluginVariable("Commands|Confirm Selection", this.m_strConfirmCommand.GetType(), this.m_strConfirmCommand));
            lstReturn.Add(new CPluginVariable("Commands|Cancel command", this.m_strCancelCommand.GetType(), this.m_strCancelCommand));
            lstReturn.Add(new CPluginVariable("Commands|Execute config command", this.m_strExecuteConfigCommand.GetType(), this.m_strExecuteConfigCommand));

			lstReturn.Add(new CPluginVariable("Banning|Ban Type", "enum.CInGameAdmin_BanType(Frostbite - Name|Frostbite - EA GUID|Punkbuster - GUID)", this.m_strBanTypeOption));

            return lstReturn;
        }

        // Lists all of the plugin variables.
        public List<CPluginVariable> GetPluginVariables() {

            List<CPluginVariable> lstReturn = new List<CPluginVariable>();

            lstReturn.Add(new CPluginVariable("Private Prefix", this.m_strPrivatePrefix.GetType(), this.m_strPrivatePrefix));
            lstReturn.Add(new CPluginVariable("Admins Prefix", this.m_strAdminsPrefix.GetType(), this.m_strAdminsPrefix));
            lstReturn.Add(new CPluginVariable("Public Prefix", this.m_strPublicPrefix.GetType(), this.m_strPublicPrefix));

            lstReturn.Add(new CPluginVariable("Kick", this.m_strKickCommand.GetType(), this.m_strKickCommand));
            lstReturn.Add(new CPluginVariable("Nuke", this.m_strNukeCommand.GetType(), this.m_strNukeCommand));
            lstReturn.Add(new CPluginVariable("Kill", this.m_strKillCommand.GetType(), this.m_strKillCommand));
            lstReturn.Add(new CPluginVariable("Move", this.m_strMoveCommand.GetType(), this.m_strMoveCommand));
            lstReturn.Add(new CPluginVariable("Force Move", this.m_strForceMoveCommand.GetType(), this.m_strForceMoveCommand));
            lstReturn.Add(new CPluginVariable("Temporary Ban", this.m_strTemporaryBanCommand.GetType(), this.m_strTemporaryBanCommand));
            lstReturn.Add(new CPluginVariable("Permanent Ban", this.m_strPermanentBanCommand.GetType(), this.m_strPermanentBanCommand));
            lstReturn.Add(new CPluginVariable("Say", this.m_strSayCommand.GetType(), this.m_strSayCommand));
            lstReturn.Add(new CPluginVariable("Player Say", this.m_strPlayerSayCommand.GetType(), this.m_strPlayerSayCommand));

            //lstReturn.Add(new CPluginVariable("Player Warn", this.m_strPlayerWarnCommand.GetType(), this.m_strPlayerWarnCommand));
            lstReturn.Add(new CPluginVariable("Restart Map", this.m_strRestartLevelCommand.GetType(), this.m_strRestartLevelCommand));
            lstReturn.Add(new CPluginVariable("Next Map", this.m_strNextLevelCommand.GetType(), this.m_strNextLevelCommand));
            lstReturn.Add(new CPluginVariable("Confirm Selection", this.m_strConfirmCommand.GetType(), this.m_strConfirmCommand));
            lstReturn.Add(new CPluginVariable("Cancel command", this.m_strCancelCommand.GetType(), this.m_strCancelCommand));
            lstReturn.Add(new CPluginVariable("Execute config command", this.m_strExecuteConfigCommand.GetType(), this.m_strExecuteConfigCommand));

			lstReturn.Add(new CPluginVariable("Ban Type", "enum.CInGameAdmin_BanType(Frostbite - Name|Frostbite - EA GUID|Punkbuster - GUID)", this.m_strBanTypeOption));

            return lstReturn;
        }