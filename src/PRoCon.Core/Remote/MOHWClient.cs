using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PRoCon.Core.Remote {
    using Core.Players;
    using Core.Maps;

    public class MOHWClient : BFClient {

        public override string GameType {
            get {
                return "MOHW";
            }
        }

        public override bool HasOpenMaplist {
            get {
                // true, does not lock maplist to a playlist
                return true;
            }
        }

        public List<MaplistEntry> lstFullMaplist {
            get;
            private set;
        }

        public List<String> lstFullReservedSlotsList {
            get;
            private set;
        }

        private int iLastMapListOffset;
        private int iLastReservedSlotsListOffset;

        public MOHWClient(FrostbiteConnection connection) : base(connection) {

            // Geoff Green 08/10/2011 - bug in BF3 OB version E, sent through with capital P.
            //this.m_requestDelegates.Add("PunkBuster.onMessage", this.DispatchPunkBusterOnMessageRequest);

            // Geoff Green 08/10/2011 - bug in BF3 OB version E, sent through as 'player.spawn'.
            this.m_requestDelegates.Add("player.spawn", this.DispatchPlayerOnSpawnRequest);

            // MoHW R-6 crippled chat
            // this.m_requestDelegates.Add("player.onChat", this.DispatchPlayerOnChatRequest);

            #region Maplist
            this.lstFullMaplist = new List<MaplistEntry>();
            this.iLastMapListOffset = 0;
            /*
            this.m_responseDelegates.Add("mapList.load", this.DispatchMapListLoadResponse);
            this.m_responseDelegates.Add("mapList.save", this.DispatchMapListSaveResponse );
            this.m_responseDelegates.Add("mapList.list", this.DispatchMapListListResponse);
            this.m_responseDelegates.Add("mapList.clear", this.DispatchMapListClearResponse);
            

            this.m_responseDelegates.Add("mapList.remove", this.DispatchMapListRemoveResponse);
            */
            this.m_responseDelegates.Add("mapList.add", this.DispatchMapListAppendResponse);

            #endregion

            #region Map list functions

            this.m_responseDelegates.Add("mapList.restartRound", this.DispatchAdminRestartRoundResponse);
            this.m_responseDelegates.Add("mapList.availableMaps", this.DispatchAdminSupportedMapsResponse);

            this.m_responseDelegates.Add("mapList.runNextRound", this.DispatchAdminRunNextRoundResponse);

            this.m_responseDelegates.Add("currentLevel", this.DispatchAdminCurrentLevelResponse);

            this.m_responseDelegates.Add("mapList.endRound", this.DispatchAdminEndRoundResponse);

            this.m_responseDelegates.Add("mapList.setNextMapIndex", this.DispatchMapListNextLevelIndexResponse);

            this.m_responseDelegates.Add("mapList.getMapIndices", this.DispatchMapListGetMapIndicesResponse);

            this.m_responseDelegates.Add("mapList.getRounds", this.DispatchMapListGetRoundsResponse);

            #endregion

            #region Reserved Slots

            this.lstFullReservedSlotsList = new List<String>();

            // Note: These delegates point to methods in FrostbiteClient.
            this.m_responseDelegates.Add("reservedSlotsList.configFile", this.DispatchReservedSlotsConfigFileResponse);
            this.m_responseDelegates.Add("reservedSlotsList.load", this.DispatchReservedSlotsLoadResponse);
            this.m_responseDelegates.Add("reservedSlotsList.save", this.DispatchReservedSlotsSaveResponse);
            this.m_responseDelegates.Add("reservedSlotsList.add", this.DispatchReservedSlotsAddPlayerResponse);
            this.m_responseDelegates.Add("reservedSlotsList.remove", this.DispatchReservedSlotsRemovePlayerResponse);
            this.m_responseDelegates.Add("reservedSlotsList.clear", this.DispatchReservedSlotsClearResponse);
            this.m_responseDelegates.Add("reservedSlotsList.list", this.DispatchReservedSlotsListResponse);
            this.m_responseDelegates.Add("reservedSlotsList.aggressiveJoin", this.DispatchReservedSlotsAggressiveJoinResponse);

            #endregion

            #region vars

            this.m_responseDelegates.Add("vars.autoBalance", this.DispatchVarsTeamBalanceResponse);
            //this.m_responseDelegates.Add("vars.noInteractivityTimeoutTime", this.DispatchVarsIdleTimeoutResponse);

            this.m_responseDelegates.Add("vars.maxPlayers", this.DispatchVarsPlayerLimitResponse);

            this.m_responseDelegates.Add("vars.3pCam", this.DispatchVarsThirdPersonVehicleCamerasResponse);

            this.m_responseDelegates.Add("admin.eventsEnabled", this.DispatchEventsEnabledResponse);

            /*
            vars.clientSideDamageArbitration
            vars.killRotation
            vars.noInteractivityRoundBan
             * */

            this.m_responseDelegates.Add("vars.vehicleSpawnAllowed", this.DispatchVarsVehicleSpawnAllowedResponse);
            this.m_responseDelegates.Add("vars.vehicleSpawnDelay", this.DispatchVarsVehicleSpawnDelayResponse);
            this.m_responseDelegates.Add("vars.bulletDamage", this.DispatchVarsBulletDamageResponse);
            this.m_responseDelegates.Add("vars.nameTag", this.DispatchVarsNameTagResponse);
            this.m_responseDelegates.Add("vars.regenerateHealth", this.DispatchVarsRegenerateHealthResponse);
            this.m_responseDelegates.Add("vars.roundRestartPlayerCount", this.DispatchVarsRoundRestartPlayerCountResponse);
            this.m_responseDelegates.Add("vars.onlySquadLeaderSpawn", this.DispatchVarsOnlySquadLeaderSpawnResponse);
            this.m_responseDelegates.Add("vars.unlockMode", this.DispatchVarsUnlockModeResponse);
            this.m_responseDelegates.Add("vars.soldierHealth", this.DispatchVarsSoldierHealthResponse);
            this.m_responseDelegates.Add("vars.hud", this.DispatchVarsHudResponse);
            this.m_responseDelegates.Add("vars.playerManDownTime", this.DispatchVarsPlayerManDownTimeResponse);
            this.m_responseDelegates.Add("vars.roundStartPlayerCount", this.DispatchVarsRoundStartPlayerCountResponse);
            this.m_responseDelegates.Add("vars.playerRespawnTime", this.DispatchVarsPlayerRespawnTimeResponse);
            this.m_responseDelegates.Add("vars.gameModeCounter", this.DispatchVarsGameModeCounterResponse);
            this.m_responseDelegates.Add("vars.idleBanRounds", this.DispatchVarsIdleBanRoundsResponse);

            this.m_responseDelegates.Add("vars.serverMessage", this.DispatchVarsServerMessageResponse);

            this.m_responseDelegates.Add("vars.roundLockdownCountdown", this.DispatchVarsRoundLockdownCountdownResponse);
            this.m_responseDelegates.Add("vars.roundWarmupTimeout", this.DispatchVarsRoundWarmupTimeoutResponse);

            this.m_responseDelegates.Add("vars.premiumStatus", this.DispatchVarsPremiumStatusResponse);

            #endregion

            #region MoHW vars

            // deprecated R-5 this.m_responseDelegates.Add("vars.allUnlocksUnlocked", this.DispatchVarsAllUnlocksUnlockedResponse);
            this.m_responseDelegates.Add("vars.buddyOutline", this.DispatchVarsBuddyOutlineResponse);
            this.m_responseDelegates.Add("vars.hudBuddyInfo", this.DispatchVarsHudBuddyInfoResponse);
            this.m_responseDelegates.Add("vars.hudClassAbility", this.DispatchVarsHudClassAbilityResponse);
            this.m_responseDelegates.Add("vars.hudCrosshair", this.DispatchVarsHudCrosshairResponse);
            this.m_responseDelegates.Add("vars.hudEnemyTag", this.DispatchVarsHudEnemyTagResponse);
            this.m_responseDelegates.Add("vars.hudExplosiveIcons", this.DispatchVarsHudExplosiveIconsResponse);
            this.m_responseDelegates.Add("vars.hudGameMode", this.DispatchVarsHudGameModeResponse);
            this.m_responseDelegates.Add("vars.hudHealthAmmo", this.DispatchVarsHudHealthAmmoResponse);
            this.m_responseDelegates.Add("vars.hudMinimap", this.DispatchVarsHudMinimapResponse);
            this.m_responseDelegates.Add("vars.hudObiturary", this.DispatchVarsHudObituraryResponse);
            this.m_responseDelegates.Add("vars.hudPointsTracker", this.DispatchVarsHudPointsTrackerResponse);
            this.m_responseDelegates.Add("vars.hudUnlocks", this.DispatchVarsHudUnlocksResponse);
            this.m_responseDelegates.Add("vars.playlist", this.DispatchVarsPlaylistResponse);
            this.m_responseDelegates.Add("plManager.setPlaylist", this.DispatchVarsPlaylistResponse);
            
            #endregion

            this.m_responseDelegates.Add("admin.help", this.DispatchHelpResponse);

            this.GetPacketsPattern = new Regex(this.GetPacketsPattern.ToString() + "|^reservedSlotsList.list", RegexOptions.Compiled);
        }
                
        public override void FetchStartupVariables() {
            // base.FetchStartupVariables();

            #region Base fetch
            this.SendGetVarsRankedPacket();
            this.SendGetVarsPlaylistPacket();
            base.FetchStartupVariablesBase();
            this.SendGetVarsTeamBalancePacket();
            this.SendGetVarsKillCamPacket();
            this.SendGetVarsThirdPersonVehicleCamerasPacket();
            #endregion

            #region MoHW global & R3 disabled
            // R-5 this.SendGetVarsAllUnlocksUnlockedPacket();
            // this.SendGetVarsVehicleSpawnAllowedPacket();
            // this.SendGetVarsVehicleSpawnDelayPacket();
            // this.SendGetVarsNameTagPacket();
            // this.SendGetVarsOnlySquadLeaderSpawnPacket();
            // this.SendGetVarsUnlockModePacket();
            // this.SendGetVarsHudPacket();
            // this.SendGetReservedSlotsListAggressiveJoinPacket();
            // this.SendGetVarsPremiumStatusPacket();
            // this.SendGetVarsRoundLockdownCountdownPacket();
            // this.SendGetVarsRoundWarmupTimeoutPacket();
            #endregion

            #region vars MoHW
            this.SendGetVarsBuddyOutlinePacket();
            this.SendGetVarsHudBuddyInfoPacket();
            this.SendGetVarsHudClassAbilityPacket();
            this.SendGetVarsHudCrosshairPacket();
            this.SendGetVarsHudEnemyTagPacket();
            this.SendGetVarsHudExplosiveIconsPacket();
            this.SendGetVarsHudGameModePacket();
            this.SendGetVarsHudHealthAmmoPacket();
            this.SendGetVarsHudMinimapPacket();
            this.SendGetVarsHudObituraryPacket();
            this.SendGetVarsHudPointsTrackerPacket();
            this.SendGetVarsHudUnlocksPacket();
            // see above this.SendGetVarsPlaylistPacket();
            #endregion

            #region BF3
            this.SendGetVarsPlayerLimitPacket();

            this.SendGetVarsIdleBanRoundsPacket();
            this.SendGetVarsBulletDamagePacket();

            this.SendGetVarsGameModeCounterPacket();

            this.SendGetVarsPlayerManDownTimePacket();
            this.SendGetVarsPlayerRespawnTimePacket();
            this.SendGetVarsRegenerateHealthPacket();

            this.SendGetVarsRoundRestartPlayerCountPacket();
            this.SendGetVarsRoundStartPlayerCountPacket();

            this.SendGetVarsServerMessagePacket();
            this.SendGetVarsSoldierHealthPacket();
            #endregion
        }

        #region Overridden Events

        public override event FrostbiteClient.ServerInfoHandler ServerInfo;

        public override event FrostbiteClient.PlayerAuthenticatedHandler PlayerAuthenticated;

        //public override event FrostbiteClient.ListPlayersHandler ListPlayers;

        //public override event FrostbiteClient.RoundOverPlayersHandler RoundOverPlayers;

        public override event FrostbiteClient.PlayerKilledHandler PlayerKilled;

        public override event FrostbiteClient.PlayerSpawnedHandler PlayerSpawned;

        // MoHW R-6 crippled chat related
        public override event FrostbiteClient.RawChatHandler Chat;
        public override event FrostbiteClient.GlobalChatHandler GlobalChat;

        #region Maplist

       // public override event FrostbiteClient.MapListConfigFileHandler MapListConfigFile;
        //public override event FrostbiteClient.EmptyParamterHandler MapListLoad;
        //public override event FrostbiteClient.EmptyParamterHandler MapListSave;
        public override event FrostbiteClient.MapListAppendedHandler MapListMapAppended;
        //public override event FrostbiteClient.MapListLevelIndexHandler MapListNextLevelIndex;
        //public override event FrostbiteClient.MapListLevelIndexHandler MapListMapRemoved;
        public override event FrostbiteClient.MapListMapInsertedHandler MapListMapInserted;
        //public override event FrostbiteClient.EmptyParamterHandler MapListCleared;
        public override event FrostbiteClient.MapListListedHandler MapListListed;

        #endregion

        #region ReservedSlotsList

        public override event FrostbiteClient.ReservedSlotsListHandler ReservedSlotsList;
        public override event FrostbiteClient.IsEnabledHandler ReservedSlotsListAggressiveJoin;

        #endregion

        #region Map/Round

        //public override event FrostbiteClient.EmptyParamterHandler RunNextRound; // Alias for runNextRound
        public override event FrostbiteClient.CurrentLevelHandler CurrentLevel;
        //public override event FrostbiteClient.EmptyParamterHandler RestartRound; // Alias for restartRound
        public override event FrostbiteClient.SupportedMapsHandler SupportedMaps;
        //public override event FrostbiteClient.EndRoundHandler EndRound;

        #endregion

        #region vars
        public override event FrostbiteClient.IsEnabledHandler VehicleSpawnAllowed;
        public override event FrostbiteClient.LimitHandler VehicleSpawnDelay;
        public override event FrostbiteClient.LimitHandler BulletDamage;
        public override event FrostbiteClient.IsEnabledHandler NameTag;
        public override event FrostbiteClient.IsEnabledHandler RegenerateHealth;
        public override event FrostbiteClient.IsEnabledHandler OnlySquadLeaderSpawn;
        public override event FrostbiteClient.UnlockModeHandler UnlockMode;
        public override event FrostbiteClient.LimitHandler SoldierHealth;
        public override event FrostbiteClient.IsEnabledHandler Hud;
        public override event FrostbiteClient.LimitHandler PlayerManDownTime;
        public override event FrostbiteClient.LimitHandler RoundRestartPlayerCount;
        public override event FrostbiteClient.LimitHandler RoundStartPlayerCount;
        public override event FrostbiteClient.LimitHandler PlayerRespawnTime;
        public override event FrostbiteClient.LimitHandler GameModeCounter;
        public override event FrostbiteClient.LimitHandler IdleBanRounds;
        public override event FrostbiteClient.ServerMessageHandler ServerMessage;
        public override event FrostbiteClient.LimitHandler RoundLockdownCountdown;
        public override event FrostbiteClient.LimitHandler RoundWarmupTimeout;
        public override event FrostbiteClient.IsEnabledHandler PremiumStatus;
        #endregion

        #region vars MoHW
        // R-5 public override event FrostbiteClient.IsEnabledHandler AllUnlocksUnlocked;
        public override event FrostbiteClient.IsEnabledHandler BuddyOutline;
        public override event FrostbiteClient.IsEnabledHandler HudBuddyInfo;
        public override event FrostbiteClient.IsEnabledHandler HudClassAbility;
        public override event FrostbiteClient.IsEnabledHandler HudCrosshair;
        public override event FrostbiteClient.IsEnabledHandler HudEnemyTag;
        public override event FrostbiteClient.IsEnabledHandler HudExplosiveIcons;
        public override event FrostbiteClient.IsEnabledHandler HudGameMode;
        public override event FrostbiteClient.IsEnabledHandler HudHealthAmmo;
        public override event FrostbiteClient.IsEnabledHandler HudMinimap;
        public override event FrostbiteClient.IsEnabledHandler HudObiturary;
        public override event FrostbiteClient.IsEnabledHandler HudPointsTracker;
        public override event FrostbiteClient.IsEnabledHandler HudUnlocks;
        public override event FrostbiteClient.PlaylistSetHandler Playlist;
        #endregion

        #endregion

        #region Overridden Packet Helpers

        public override void SendEventsEnabledPacket(bool isEventsEnabled) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("admin.eventsEnabled", Packet.bltos(isEventsEnabled));
            }
        }

        public override void SendHelpPacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("admin.help");
            }
        }

        #region Map Controls

        public override void SendAdminRestartRoundPacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("mapList.restartRound");
            }
        }

        public override void SendAdminRunNextRoundPacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("mapList.runNextRound");
            }
        }

        public override void SendMapListNextLevelIndexPacket(int index) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("mapList.setNextMapIndex", index.ToString());
            }
        }
        
        #endregion

        #region Maplist
        
        public override void SendMapListListRoundsPacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("mapList.list");
            }
        }

        public virtual void SendMapListListRoundsPacket(int startIndex) {
            if (this.IsLoggedIn == true) {
                if (startIndex >= 0) {
                    this.BuildSendPacket("mapList.list", startIndex.ToString());
                } else {
                    this.BuildSendPacket("mapList.list");
                }
            }
        }

        /*
        public override void SendMapListClearPacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("mapList.clear");
            }
        }

        public override void SendMapListSavePacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("mapList.save");
            }
        }
        */
        public override void SendMapListAppendPacket(MaplistEntry map) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("mapList.add", map.MapFileName, map.Gamemode, (map.Rounds > 0 ? map.Rounds : 2).ToString());
            }
        }

        // Find default 
        /*

        public override void SendMapListRemovePacket(int index) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("mapList.remove", index.ToString());
            }
        }
        */
        public override void SendMapListInsertPacket(MaplistEntry map) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("mapList.add", map.MapFileName, map.Gamemode, (map.Rounds > 0 ? map.Rounds : 2).ToString(), map.Index.ToString());
            }
        }

        #endregion

        #region Reserved Slot List

        public override void SendReservedSlotsLoadPacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("reservedSlotsList.load");
            }
        }

        public override void SendReservedSlotsListPacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("reservedSlotsList.list");
            }
        }

        public virtual void SendReservedSlotsListPacket(int startIndex) {
            if (this.IsLoggedIn == true) {
                if (startIndex >= 0) {
                    this.BuildSendPacket("reservedSlotsList.list", startIndex.ToString());
                } else {
                    this.BuildSendPacket("reservedSlotsList.list");
                }
            }
        }

        public override void SendReservedSlotsAddPlayerPacket(string soldierName) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("reservedSlotsList.add", soldierName);
            }
        }

        public override void SendReservedSlotsRemovePlayerPacket(string soldierName) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("reservedSlotsList.remove", soldierName);
            }
        }

        public override void SendReservedSlotsSavePacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("reservedSlotsList.save");
            }
        }

        public virtual void SendReservedSlotsAggressiveJoinPacket(bool enabled) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("reservedSlotsList.aggressiveJoin", Packet.bltos(enabled));
            }
        }

        #endregion

        #region Vars
        #region BF3 & MoHW
        /*
        public override void SendSetVarsIdleTimeoutPacket(int limit) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.noInteractivityTimeoutTime", limit.ToString());
            }
        }

        public override void SendGetVarsIdleTimeoutPacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.noInteractivityTimeoutTime");
            }
        }
        */
        public override void SendSetVarsPlayerLimitPacket(int limit) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.maxPlayers", limit.ToString());
            }
        }

        public override void SendGetVarsPlayerLimitPacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.maxPlayers");
            }
        }

        public override void SendSetVarsTeamBalancePacket(bool enabled) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.autoBalance", Packet.bltos(enabled));
            }
        }


        public override void SendGetVarsTeamBalancePacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.autoBalance");
            }
        }

        public override void SendSetVarsThirdPersonVehicleCamerasPacket(bool enabled) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.3pCam", Packet.bltos(enabled));
            }
        }

        public override void SendGetVarsThirdPersonVehicleCamerasPacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.3pCam");
            }
        }


        public override void SendSetVarsAdminPasswordPacket(string password) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("admin.password", password);
            }
        }

        public override void SendGetVarsAdminPasswordPacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("admin.password");
            }
        }

        public override void SendSetVarsRoundLockdownCountdownPacket(int limit) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.roundLockdownCountdown", limit.ToString());
            }
        }

        public override void SendGetVarsRoundLockdownCountdownPacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.roundLockdownCountdown");
            }
        }

        public override void SendSetVarsRoundWarmupTimeoutPacket(int limit) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.roundWarmupTimeout", limit.ToString());
            }
        }

        public override void SendGetVarsRoundWarmupTimeoutPacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.roundWarmupTimeout");
            }
        }

        public virtual void SendPremiumStatusPacket(bool enabled)
        {
            if (this.IsLoggedIn == true)
            {
                this.BuildSendPacket("vars.premiumStatus", Packet.bltos(enabled));
            }
        }

        #endregion

        #region MoHW only
        /* deprecated R-5
        // VarsAllUnlocksUnlocked
        public override void SendSetVarsAllUnlocksUnlockedPacket(bool enabled) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.allUnlocksUnlocked", Packet.bltos(enabled));
            }
        }

        public override void SendGetVarsAllUnlocksUnlockedPacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.allUnlocksUnlocked");
            }
        }
        */
        // VarsHudBuddyOutline
        public override void SendSetVarsBuddyOutlinePacket(bool enabled) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.buddyOutline", Packet.bltos(enabled));
            }
        }

        public override void SendGetVarsBuddyOutlinePacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.buddyOutline");
            }
        }
        // VarsHudBuddyInfo
        public override void SendSetVarsHudBuddyInfoPacket(bool enabled) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.hudBuddyInfo", Packet.bltos(enabled));
            }
        }

        public override void SendGetVarsHudBuddyInfoPacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.hudBuddyInfo");
            }
        }
        // VarsHudClassAbility
        public override void SendSetVarsHudClassAbilityPacket(bool enabled) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.hudClassAbility", Packet.bltos(enabled));
            }
        }

        public override void SendGetVarsHudClassAbilityPacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.hudClassAbility");
            }
        }
        // VarsHudCrosshair
        public override void SendSetVarsHudCrosshairPacket(bool enabled) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.hudCrosshair", Packet.bltos(enabled));
            }
        }

        public override void SendGetVarsHudCrosshairPacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.hudCrosshair");
            }
        }
        // VarsHudEnemyTag
        public override void SendSetVarsHudEnemyTagPacket(bool enabled) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.hudEnemyTag", Packet.bltos(enabled));
            }
        }

        public override void SendGetVarsHudEnemyTagPacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.hudEnemyTag");
            }
        }
        // VarsHudExplosiveIcons
        public override void SendSetVarsHudExplosiveIconsPacket(bool enabled) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.hudExplosiveIcons", Packet.bltos(enabled));
            }
        }

        public override void SendGetVarsHudExplosiveIconsPacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.hudExplosiveIcons");
            }
        }
        // VarsHudGameMode
        public override void SendSetVarsHudGameModePacket(bool enabled) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.hudGameMode", Packet.bltos(enabled));
            }
        }

        public override void SendGetVarsHudGameModePacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.hudGameMode");
            }
        }
        // VarsHudHealthAmmo
        public override void SendSetVarsHudHealthAmmoPacket(bool enabled) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.hudHealthAmmo", Packet.bltos(enabled));
            }
        }

        public override void SendGetVarsHudHealthAmmoPacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.hudHealthAmmo");
            }
        }
        // VarsHudMinimapResponse
        public override void SendSetVarsHudMinimapPacket(bool enabled) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.hudMinimap", Packet.bltos(enabled));
            }
        }

        public override void SendGetVarsHudMinimapPacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.hudMinimap");
            }
        }
        // VarsHudObiturary
        public override void SendSetVarsHudObituraryPacket(bool enabled) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.hudObiturary", Packet.bltos(enabled));
            }
        }

        public override void SendGetVarsHudObituraryPacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.hudObiturary");
            }
        }
        // VarsHudPointsTracker
        public override void SendSetVarsHudPointsTrackerPacket(bool enabled) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.hudPointsTracker", Packet.bltos(enabled));
            }
        }

        public override void SendGetVarsHudPointsTrackerPacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.hudPointsTracker");
            }
        }
        // VarsHudUnlocks
        public override void SendSetVarsHudUnlocksPacket(bool enabled) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.hudUnlocks", Packet.bltos(enabled));
            }
        }

        public override void SendGetVarsHudUnlocksPacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.hudUnlocks");
            }
        }
        // VarsPlaylist
        public override void SendSetVarsPlaylistPacket(string playlist) {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.playlist", playlist);
            }
        }

        public override void SendGetVarsPlaylistPacket() {
            if (this.IsLoggedIn == true) {
                this.BuildSendPacket("vars.playlist");
            }
        }

        #endregion
        
        #endregion

        #region Level Vars (Non existent in BF3)

        public override void SendLevelVarsListPacket(LevelVariableContext context) {
            // Do nothing!
        }

        #endregion

        #endregion

        #region Overridden Response Handlers

        protected override void DispatchPlayerOnJoinRequest(FrostbiteConnection sender, Packet cpRequestPacket) {
            base.DispatchPlayerOnJoinRequest(sender, cpRequestPacket);

            if (cpRequestPacket.Words.Count >= 3) {
                if (this.PlayerAuthenticated != null) {
                    FrostbiteConnection.RaiseEvent(this.PlayerAuthenticated.GetInvocationList(), this, cpRequestPacket.Words[1], cpRequestPacket.Words[2]);
                }
            }
        }

        protected override void DispatchServerInfoResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {

            if (this.ServerInfo != null) {

                CServerInfo newServerInfo = new CServerInfo(
                    new List<string>() {
                        "ServerName",
                        "PlayerCount",
                        "MaxPlayerCount",
                        "GameMode",
                        "Map",
                        "CurrentRound",
                        "TotalRounds",
                        "TeamScores",
                        "ConnectionState",
                        "Ranked",
                        "PunkBuster",
                        "Passworded",
                        "ServerUptime",
                        "RoundTime",
                        "GameMod", // Note: if another variable is affixed to both games this method
                        // "Mappack", // will need to be split into MoHClient and BFBC2Client.
                        // "ExternalGameIpandPort",
                        "PunkBusterVersion",
                        "Placeholder01",
                        "ServerRegion",
                        "PingSite",
                        "ServerCountry",
                        "JoinQueueEnabled"
                    }, cpRecievedPacket.Words.GetRange(1, cpRecievedPacket.Words.Count - 1)
                );

                FrostbiteConnection.RaiseEvent(this.ServerInfo.GetInvocationList(), this, newServerInfo);
            }
        }
        /*
        protected override void DispatchAdminListPlayersResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 2) {

                cpRecievedPacket.Words.RemoveAt(0);
                if (this.ListPlayers != null) {

                    List<CPlayerInfo> lstPlayers = BF3PlayerInfo.GetPlayerList(cpRecievedPacket.Words);
                    CPlayerSubset cpsSubset = new CPlayerSubset(cpRequestPacket.Words.GetRange(1, cpRequestPacket.Words.Count - 1));

                    FrostbiteConnection.RaiseEvent(this.ListPlayers.GetInvocationList(), this, lstPlayers, cpsSubset);
                }
            }
        }
        
        protected override void DispatchServerOnRoundOverPlayersRequest(FrostbiteConnection sender, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 2) {

                cpRequestPacket.Words.RemoveAt(0);
                if (this.RoundOverPlayers != null) {
                    FrostbiteConnection.RaiseEvent(this.RoundOverPlayers.GetInvocationList(), this, BF3PlayerInfo.GetPlayerList(cpRequestPacket.Words));
                }

            }
        }
        */
        protected override void DispatchPlayerOnKillRequest(FrostbiteConnection sender, Packet cpRequestPacket) {
            bool blHeadshot = false;
            if (this.PlayerKilled != null) {

                if (bool.TryParse(cpRequestPacket.Words[4], out blHeadshot) == true) {
                    FrostbiteConnection.RaiseEvent(this.PlayerKilled.GetInvocationList(), this, cpRequestPacket.Words[1], cpRequestPacket.Words[2], cpRequestPacket.Words[3], blHeadshot, new Point3D(0, 0, 0), new Point3D(0, 0, 0));
                }
            }

        }

        protected override void DispatchPlayerOnSpawnRequest(FrostbiteConnection sender, Packet cpRequestPacket) {
            //if (cpRequestPacket.Words.Count >= 9) {
                if (this.PlayerSpawned != null) {
                    FrostbiteConnection.RaiseEvent(this.PlayerSpawned.GetInvocationList(), this, cpRequestPacket.Words[1], "", new List<string>() { "", "", "" }, new List<string>() { "", "", "" }); // new Inventory(cpRequestPacket.Words[3], cpRequestPacket.Words[4], cpRequestPacket.Words[5], cpRequestPacket.Words[6], cpRequestPacket.Words[7], cpRequestPacket.Words[8]));
                }
            //}
        }

        // obsolet since R-21 BF3 but back in with MoHW R-6: This can probably be removed once the onChat event is un-lamed.
        protected override void DispatchPlayerOnChatRequest(FrostbiteConnection sender, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 3) {
                if (this.GlobalChat != null) {
                    FrostbiteConnection.RaiseEvent(this.GlobalChat.GetInvocationList(), this, cpRequestPacket.Words[1], cpRequestPacket.Words[2]);
                }

                cpRequestPacket.Words.RemoveAt(0);
                if (this.Chat != null) {
                    FrostbiteConnection.RaiseEvent(this.Chat.GetInvocationList(), this, cpRequestPacket.Words);
                }
            }
        }

        #region Map Functions
        /*
        protected override void DispatchAdminRunNextRoundResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.RunNextRound != null) {
                    FrostbiteConnection.RaiseEvent(this.RunNextRound.GetInvocationList(), this);
                }
            }
        }
        */
        protected override void DispatchAdminCurrentLevelResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1 && cpRecievedPacket.Words.Count >= 2) {
                if (this.CurrentLevel != null) {
                    FrostbiteConnection.RaiseEvent(this.CurrentLevel.GetInvocationList(), this, cpRecievedPacket.Words[1]);
                }
            }
        }
        /*
        protected override void DispatchAdminRestartRoundResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.RestartRound != null) {
                    FrostbiteConnection.RaiseEvent(this.RestartRound.GetInvocationList(), this);
                }
            }
        }
        */
        protected override void DispatchAdminSupportedMapsResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            /*
            if (cpRequestPacket.Words.Count >= 2 && cpRecievedPacket.Words.Count > 1) {
                if (this.SupportedMaps != null) {
                    FrostbiteConnection.RaiseEvent(this.SupportedMaps.GetInvocationList(), this, cpRequestPacket.Words[1], cpRecievedPacket.Words.GetRange(1, cpRecievedPacket.Words.Count - 1));
                }
            }
            */
        }

        #endregion

        #region Map List
        /*
        protected override void DispatchMapListLoadResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.MapListLoad != null) {
                    FrostbiteConnection.RaiseEvent(this.MapListLoad.GetInvocationList(), this);
                }
            }
        }

        protected override void DispatchMapListSaveResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.MapListSave != null) {
                    FrostbiteConnection.RaiseEvent(this.MapListSave.GetInvocationList(), this);
                }
            }
        }
        */
        protected override void DispatchMapListListResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {

                // start try for mapList.list offset fetch
                int iRequestStartOffset = 0;
                int iNumReturnedMaps = 0;

                if (cpRequestPacket.Words.Count >= 2) {
                    if (int.TryParse(cpRequestPacket.Words[1], out iRequestStartOffset) == false) {
                        iRequestStartOffset = 0;
                    }
                }

                if (int.TryParse(cpRecievedPacket.Words[0], out iNumReturnedMaps) == false) {
                    iNumReturnedMaps = 0;
                }
                // end

                cpRecievedPacket.Words.RemoveAt(0);
                cpRecievedPacket.Words.RemoveAt(1); // quick hack for additional playlist info parameter (R-5)

                if (iRequestStartOffset == 0) {
                    this.lstFullMaplist.Clear();
                    this.iLastMapListOffset = 0;
                } else {
                    if (this.iLastMapListOffset < iRequestStartOffset) {
                        this.iLastMapListOffset = iRequestStartOffset;
                    } else {
                        return;
                    }
                }

                #region parse mapList.List output
                List<MaplistEntry> lstMaplist = new List<MaplistEntry>();

                int maps = 0;
                int parameters = 0;

                if (int.TryParse(cpRecievedPacket.Words[0], out maps) == true && int.TryParse(cpRecievedPacket.Words[1], out parameters) == true) {

                    for (int mapOffset = 0; mapOffset < maps; mapOffset++) {

                        string mapFileName = "";
                        string gameMode = "";
                        int rounds = 0;

                        for (int parameterOffset = 0; parameterOffset < parameters; parameterOffset++) {

                            string data = cpRecievedPacket.Words[2 + (mapOffset * parameters) + parameterOffset];

                            switch (parameterOffset) {
                                case 0:
                                    Match match = Regex.Match(data, @"MOHW\/Levels\/MP\/.*?\/(.*)");
                                    if (match.Success) {
                                        data = match.Groups[1].Value;
                                    }
                                    // orginal
                                    mapFileName = data;
                                    break;
                                case 1:
                                    gameMode = data;
                                    break;
                                case 2:
                                    int.TryParse(data, out rounds);
                                    break;
                                default:
                                    break;
                            }
                        }

                        lstMaplist.Add(new MaplistEntry(gameMode, mapFileName, rounds));
                    }

                    #region oldStuff
                    /*
                    int rounds = 0;

                    for (int i = 0; i < cpRecievedPacket.Words.Count; i = i + 3) {
                        if (int.TryParse(cpRecievedPacket.Words[i + 2], out rounds) == true) {

                            Match temp = Regex.Match(cpRecievedPacket.Words[i], @"Levels\/.*?\/(.*)");

                            lstMaplist.Add(new MaplistEntry(cpRecievedPacket.Words[i + 1], temp.Groups[1].Value, rounds));
                        }
                    }
                    */
                    #endregion // oldStuff
                }
                #endregion //parse mapList.List output

                if (lstMaplist.Count > 0) {
                    this.lstFullMaplist.AddRange(lstMaplist);
                    /* quick MOHW hack mapList.list
                    this.SendMapListListRoundsPacket(iRequestStartOffset + 100);
                }
                else { */
                    // original
                    if (this.MapListListed != null)
                    {
                        FrostbiteConnection.RaiseEvent(this.MapListListed.GetInvocationList(), this, this.lstFullMaplist);
                    }
                }
            }
        }
        /*
        protected override void DispatchMapListClearResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.MapListCleared != null) {
                    FrostbiteConnection.RaiseEvent(this.MapListCleared.GetInvocationList(), this);
                }
            }
        }
        */
        protected override void DispatchMapListAppendResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 4) {
                MaplistEntry mapEntry = null;

                int rounds = 0;
                int index = -1;

                if (int.TryParse(cpRequestPacket.Words[3], out rounds) == true) {
                    mapEntry = new MaplistEntry(cpRequestPacket.Words[2], cpRequestPacket.Words[1], rounds);
                }

                if (cpRequestPacket.Words.Count >= 5 && int.TryParse(cpRequestPacket.Words[4], out index) == true) {
                    mapEntry = new MaplistEntry(cpRequestPacket.Words[2], cpRequestPacket.Words[1], rounds, index);
                }

                if (this.MapListMapAppended != null) {
                    if (index == -1) {
                        FrostbiteConnection.RaiseEvent(this.MapListMapAppended.GetInvocationList(), this, mapEntry);
                    }
                    else {
                        FrostbiteConnection.RaiseEvent(this.MapListMapInserted.GetInvocationList(), this, mapEntry);
                    }
                }
            }
        }
        /*
        protected override void DispatchMapListNextLevelIndexResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {

                int iMapIndex = 0;
                if (this.MapListNextLevelIndex != null) {
                    if ((cpRequestPacket.Words.Count >= 2 && int.TryParse(cpRequestPacket.Words[1], out iMapIndex) == true) || cpRecievedPacket.Words.Count >= 2 && int.TryParse(cpRecievedPacket.Words[1], out iMapIndex) == true) {
                        FrostbiteConnection.RaiseEvent(this.MapListNextLevelIndex.GetInvocationList(), this, iMapIndex);
                    }
                }
            }
        }

        protected override void DispatchMapListRemoveResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 2) {

                int iMapIndex = 0;
                if (int.TryParse(cpRequestPacket.Words[1], out iMapIndex) == true) {
                    if (this.MapListMapRemoved != null) {
                        FrostbiteConnection.RaiseEvent(this.MapListMapRemoved.GetInvocationList(), this, iMapIndex);
                    }
                }
            }
        }
        */
        protected override void DispatchMapListInsertResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 4) {

                int iMapIndex = 0, iRounds = 0;
                if (int.TryParse(cpRequestPacket.Words[1], out iMapIndex) == true && int.TryParse(cpRequestPacket.Words[3], out iRounds) == true) {
                    if (this.MapListMapInserted != null) {
                        FrostbiteConnection.RaiseEvent(this.MapListMapInserted.GetInvocationList(), this, iMapIndex, cpRequestPacket.Words[2], iRounds);
                    }
                }
            }
        }

        #endregion

        #region reservedSlotsList.list

        protected override void DispatchReservedSlotsListResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1)
            {
                int iRequestStartOffset = 0;

                if (cpRequestPacket.Words.Count >= 2)
                {
                    if (int.TryParse(cpRequestPacket.Words[1], out iRequestStartOffset) == false)
                    {
                        iRequestStartOffset = 0;
                    }
                }
                //
                /* if (iRequestStartOffset == 0)
                {
                    this.lstFullReservedSlotsList.Clear();
                } */
                if (iRequestStartOffset == 0) {
                    this.lstFullReservedSlotsList.Clear();
                    this.iLastReservedSlotsListOffset = 0;
                } else {
                    if (this.iLastReservedSlotsListOffset < iRequestStartOffset) {
                        this.iLastReservedSlotsListOffset = iRequestStartOffset;
                    } else {
                        return;
                    }
                }

                cpRecievedPacket.Words.RemoveAt(0);
                // List<String> lstReservedSlotsList = new List<String>();

                if (cpRecievedPacket.Words.Count > 0)
                {
                    this.lstFullReservedSlotsList.AddRange(cpRecievedPacket.Words);
                    /* quick MoHW hack reservedSlotsList.list
                    this.SendReservedSlotsListPacket(iRequestStartOffset + 100);
                }
                else
                { */
                    // original
                    if (this.ReservedSlotsList != null)
                    {
                        FrostbiteConnection.RaiseEvent(this.ReservedSlotsList.GetInvocationList(), this, this.lstFullReservedSlotsList);
                    }
                }
            }
        }

        #endregion

        #region Vars
        #region BF3 shared
        protected virtual void DispatchVarsBulletDamageResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.BulletDamage != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        FrostbiteConnection.RaiseEvent(this.BulletDamage.GetInvocationList(), this, Convert.ToInt32(cpRecievedPacket.Words[1]));
                    }
                    else if (cpRequestPacket.Words.Count >= 2) {
                        FrostbiteConnection.RaiseEvent(this.BulletDamage.GetInvocationList(), this, Convert.ToInt32(cpRequestPacket.Words[1]));
                    }
                }
            }
        }


        protected virtual void DispatchVarsSoldierHealthResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.SoldierHealth != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        FrostbiteConnection.RaiseEvent(this.SoldierHealth.GetInvocationList(), this, Convert.ToInt32(cpRecievedPacket.Words[1]));
                    }
                    else if (cpRequestPacket.Words.Count >= 2) {
                        FrostbiteConnection.RaiseEvent(this.SoldierHealth.GetInvocationList(), this, Convert.ToInt32(cpRequestPacket.Words[1]));
                    }
                }
            }
        }

        protected virtual void DispatchVarsPlayerManDownTimeResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.PlayerManDownTime != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        FrostbiteConnection.RaiseEvent(this.PlayerManDownTime.GetInvocationList(), this, Convert.ToInt32(cpRecievedPacket.Words[1]));
                    }
                    else if (cpRequestPacket.Words.Count >= 2) {
                        FrostbiteConnection.RaiseEvent(this.PlayerManDownTime.GetInvocationList(), this, Convert.ToInt32(cpRequestPacket.Words[1]));
                    }
                }
            }
        }

        protected virtual void DispatchVarsRoundRestartPlayerCountResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.RoundRestartPlayerCount != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        FrostbiteConnection.RaiseEvent(this.RoundRestartPlayerCount.GetInvocationList(), this, Convert.ToInt32(cpRecievedPacket.Words[1]));
                    }
                    else if (cpRequestPacket.Words.Count >= 2) {
                        FrostbiteConnection.RaiseEvent(this.RoundRestartPlayerCount.GetInvocationList(), this, Convert.ToInt32(cpRequestPacket.Words[1]));
                    }
                }
            }
        }

        protected virtual void DispatchVarsRoundStartPlayerCountResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.RoundStartPlayerCount != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        FrostbiteConnection.RaiseEvent(this.RoundStartPlayerCount.GetInvocationList(), this, Convert.ToInt32(cpRecievedPacket.Words[1]));
                    }
                    else if (cpRequestPacket.Words.Count >= 2) {
                        FrostbiteConnection.RaiseEvent(this.RoundStartPlayerCount.GetInvocationList(), this, Convert.ToInt32(cpRequestPacket.Words[1]));
                    }
                }
            }
        }

        protected virtual void DispatchVarsPlayerRespawnTimeResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.PlayerRespawnTime != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        FrostbiteConnection.RaiseEvent(this.PlayerRespawnTime.GetInvocationList(), this, Convert.ToInt32(cpRecievedPacket.Words[1]));
                    }
                    else if (cpRequestPacket.Words.Count >= 2) {
                        FrostbiteConnection.RaiseEvent(this.PlayerRespawnTime.GetInvocationList(), this, Convert.ToInt32(cpRequestPacket.Words[1]));
                    }
                }
            }
        }

        protected virtual void DispatchVarsGameModeCounterResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket)
        {
            if (cpRequestPacket.Words.Count >= 1)
            {
                if (this.GameModeCounter != null)
                {
                    if (cpRecievedPacket.Words.Count == 2)
                    {
                        FrostbiteConnection.RaiseEvent(this.GameModeCounter.GetInvocationList(), this, Convert.ToInt32(cpRecievedPacket.Words[1]));
                    }
                    else if (cpRequestPacket.Words.Count >= 2)
                    {
                        FrostbiteConnection.RaiseEvent(this.GameModeCounter.GetInvocationList(), this, Convert.ToInt32(cpRequestPacket.Words[1]));
                    }
                }
            }
        }

        protected virtual void DispatchVarsIdleBanRoundsResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket)
        {
            if (cpRequestPacket.Words.Count >= 1)
            {
                if (this.IdleBanRounds != null)
                {
                    if (cpRecievedPacket.Words.Count == 2)
                    {
                        FrostbiteConnection.RaiseEvent(this.IdleBanRounds.GetInvocationList(), this, Convert.ToInt32(cpRecievedPacket.Words[1]));
                    }
                    else if (cpRequestPacket.Words.Count >= 2)
                    {
                        FrostbiteConnection.RaiseEvent(this.IdleBanRounds.GetInvocationList(), this, Convert.ToInt32(cpRequestPacket.Words[1]));
                    }
                }
            }
        }

        protected virtual void DispatchVarsVehicleSpawnAllowedResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket)
        {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.VehicleSpawnAllowed != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        FrostbiteConnection.RaiseEvent(this.VehicleSpawnAllowed.GetInvocationList(), this, Convert.ToBoolean(cpRecievedPacket.Words[1]));
                    }
                    else if (cpRequestPacket.Words.Count >= 2) {
                        FrostbiteConnection.RaiseEvent(this.VehicleSpawnAllowed.GetInvocationList(), this, Convert.ToBoolean(cpRequestPacket.Words[1]));
                    }
                }
            }
        }


        protected virtual void DispatchVarsVehicleSpawnDelayResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.VehicleSpawnDelay != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        FrostbiteConnection.RaiseEvent(this.VehicleSpawnDelay.GetInvocationList(), this, Convert.ToInt32(cpRecievedPacket.Words[1]));
                    }
                    else if (cpRequestPacket.Words.Count >= 2) {
                        FrostbiteConnection.RaiseEvent(this.VehicleSpawnDelay.GetInvocationList(), this, Convert.ToInt32(cpRequestPacket.Words[1]));
                    }
                }
            }
        }

        protected virtual void DispatchVarsNameTagResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.NameTag != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        FrostbiteConnection.RaiseEvent(this.NameTag.GetInvocationList(), this, Convert.ToBoolean(cpRecievedPacket.Words[1]));
                    }
                    else if (cpRequestPacket.Words.Count >= 2) {
                        FrostbiteConnection.RaiseEvent(this.NameTag.GetInvocationList(), this, Convert.ToBoolean(cpRequestPacket.Words[1]));
                    }
                }
            }
        }

        protected virtual void DispatchVarsRegenerateHealthResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.RegenerateHealth != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        FrostbiteConnection.RaiseEvent(this.RegenerateHealth.GetInvocationList(), this, Convert.ToBoolean(cpRecievedPacket.Words[1]));
                    }
                    else if (cpRequestPacket.Words.Count >= 2) {
                        FrostbiteConnection.RaiseEvent(this.RegenerateHealth.GetInvocationList(), this, Convert.ToBoolean(cpRequestPacket.Words[1]));
                    }
                }
            }
        }

        protected virtual void DispatchVarsOnlySquadLeaderSpawnResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.OnlySquadLeaderSpawn != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        FrostbiteConnection.RaiseEvent(this.OnlySquadLeaderSpawn.GetInvocationList(), this, Convert.ToBoolean(cpRecievedPacket.Words[1]));
                    }
                    else if (cpRequestPacket.Words.Count >= 2) {
                        FrostbiteConnection.RaiseEvent(this.OnlySquadLeaderSpawn.GetInvocationList(), this, Convert.ToBoolean(cpRequestPacket.Words[1]));
                    }
                }
            }
        }

        protected virtual void DispatchVarsUnlockModeResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket)
        {
            if (cpRequestPacket.Words.Count >= 1)
            {
                if (this.UnlockMode != null)
                {
                    if (cpRecievedPacket.Words.Count == 2)
                    {
                        FrostbiteConnection.RaiseEvent(this.UnlockMode.GetInvocationList(), this, Convert.ToString(cpRecievedPacket.Words[1]));
                    }
                    else if (cpRequestPacket.Words.Count >= 2)
                    {
                        FrostbiteConnection.RaiseEvent(this.UnlockMode.GetInvocationList(), this, Convert.ToString(cpRequestPacket.Words[1]));
                    }
                }
            }
        }

        protected virtual void DispatchVarsHudResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.Hud != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        FrostbiteConnection.RaiseEvent(this.Hud.GetInvocationList(), this, Convert.ToBoolean(cpRecievedPacket.Words[1]));
                    }
                    else if (cpRequestPacket.Words.Count >= 2) {
                        FrostbiteConnection.RaiseEvent(this.Hud.GetInvocationList(), this, Convert.ToBoolean(cpRequestPacket.Words[1]));
                    }
                }
            }
        }

        protected virtual void DispatchVarsServerMessageResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket)
        {
            if (cpRequestPacket.Words.Count >= 1)
            {
                if (this.ServerMessage != null)
                {
                    if (cpRecievedPacket.Words.Count == 2)
                    {
                        FrostbiteConnection.RaiseEvent(this.ServerMessage.GetInvocationList(), this, cpRecievedPacket.Words[1]);
                    }
                    else if (cpRequestPacket.Words.Count >= 2)
                    {
                        FrostbiteConnection.RaiseEvent(this.ServerMessage.GetInvocationList(), this, cpRequestPacket.Words[1]);
                    }
                }
            }
        }
        #endregion

        #region MoHW
        /* deprecated R-5
        protected virtual void DispatchVarsAllUnlocksUnlockedResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.AllUnlocksUnlocked != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        FrostbiteConnection.RaiseEvent(this.AllUnlocksUnlocked.GetInvocationList(), this, Convert.ToBoolean(cpRecievedPacket.Words[1]));
                    } else if (cpRequestPacket.Words.Count >= 2) {
                        FrostbiteConnection.RaiseEvent(this.AllUnlocksUnlocked.GetInvocationList(), this, Convert.ToBoolean(cpRequestPacket.Words[1]));
                    }
                }
            }
        }
        */
        protected virtual void DispatchVarsBuddyOutlineResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.BuddyOutline != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        FrostbiteConnection.RaiseEvent(this.BuddyOutline.GetInvocationList(), this, Convert.ToBoolean(cpRecievedPacket.Words[1]));
                    }
                    else if (cpRequestPacket.Words.Count >= 2) {
                        FrostbiteConnection.RaiseEvent(this.BuddyOutline.GetInvocationList(), this, Convert.ToBoolean(cpRequestPacket.Words[1]));
                    }
                }
            }
        }

        protected virtual void DispatchVarsHudBuddyInfoResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.HudBuddyInfo != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        FrostbiteConnection.RaiseEvent(this.HudBuddyInfo.GetInvocationList(), this, Convert.ToBoolean(cpRecievedPacket.Words[1]));
                    } else if (cpRequestPacket.Words.Count >= 2) {
                        FrostbiteConnection.RaiseEvent(this.HudBuddyInfo.GetInvocationList(), this, Convert.ToBoolean(cpRequestPacket.Words[1]));
                    }
                }
            }
        }

        protected virtual void DispatchVarsHudClassAbilityResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.HudClassAbility != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        FrostbiteConnection.RaiseEvent(this.HudClassAbility.GetInvocationList(), this, Convert.ToBoolean(cpRecievedPacket.Words[1]));
                    } else if (cpRequestPacket.Words.Count >= 2) {
                        FrostbiteConnection.RaiseEvent(this.HudClassAbility.GetInvocationList(), this, Convert.ToBoolean(cpRequestPacket.Words[1]));
                    }
                }
            }
        }

        protected virtual void DispatchVarsHudCrosshairResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.HudCrosshair != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        FrostbiteConnection.RaiseEvent(this.HudCrosshair.GetInvocationList(), this, Convert.ToBoolean(cpRecievedPacket.Words[1]));
                    } else if (cpRequestPacket.Words.Count >= 2) {
                        FrostbiteConnection.RaiseEvent(this.HudCrosshair.GetInvocationList(), this, Convert.ToBoolean(cpRequestPacket.Words[1]));
                    }
                }
            }
        }

        protected virtual void DispatchVarsHudEnemyTagResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.HudEnemyTag != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        FrostbiteConnection.RaiseEvent(this.HudEnemyTag.GetInvocationList(), this, Convert.ToBoolean(cpRecievedPacket.Words[1]));
                    } else if (cpRequestPacket.Words.Count >= 2) {
                        FrostbiteConnection.RaiseEvent(this.HudEnemyTag.GetInvocationList(), this, Convert.ToBoolean(cpRequestPacket.Words[1]));
                    }
                }
            }
        }

        protected virtual void DispatchVarsHudExplosiveIconsResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.HudExplosiveIcons != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        FrostbiteConnection.RaiseEvent(this.HudExplosiveIcons.GetInvocationList(), this, Convert.ToBoolean(cpRecievedPacket.Words[1]));
                    } else if (cpRequestPacket.Words.Count >= 2) {
                        FrostbiteConnection.RaiseEvent(this.HudExplosiveIcons.GetInvocationList(), this, Convert.ToBoolean(cpRequestPacket.Words[1]));
                    }
                }
            }
        }

        protected virtual void DispatchVarsHudGameModeResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.HudGameMode != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        FrostbiteConnection.RaiseEvent(this.HudGameMode.GetInvocationList(), this, Convert.ToBoolean(cpRecievedPacket.Words[1]));
                    } else if (cpRequestPacket.Words.Count >= 2) {
                        FrostbiteConnection.RaiseEvent(this.HudGameMode.GetInvocationList(), this, Convert.ToBoolean(cpRequestPacket.Words[1]));
                    }
                }
            }
        }

        protected virtual void DispatchVarsHudHealthAmmoResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.HudHealthAmmo != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        FrostbiteConnection.RaiseEvent(this.HudHealthAmmo.GetInvocationList(), this, Convert.ToBoolean(cpRecievedPacket.Words[1]));
                    } else if (cpRequestPacket.Words.Count >= 2) {
                        FrostbiteConnection.RaiseEvent(this.HudHealthAmmo.GetInvocationList(), this, Convert.ToBoolean(cpRequestPacket.Words[1]));
                    }
                }
            }
        }

        protected virtual void DispatchVarsHudMinimapResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.HudMinimap != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        FrostbiteConnection.RaiseEvent(this.HudMinimap.GetInvocationList(), this, Convert.ToBoolean(cpRecievedPacket.Words[1]));
                    } else if (cpRequestPacket.Words.Count >= 2) {
                        FrostbiteConnection.RaiseEvent(this.HudMinimap.GetInvocationList(), this, Convert.ToBoolean(cpRequestPacket.Words[1]));
                    }
                }
            }
        }

        protected virtual void DispatchVarsHudObituraryResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.HudObiturary != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        FrostbiteConnection.RaiseEvent(this.HudObiturary.GetInvocationList(), this, Convert.ToBoolean(cpRecievedPacket.Words[1]));
                    } else if (cpRequestPacket.Words.Count >= 2) {
                        FrostbiteConnection.RaiseEvent(this.HudObiturary.GetInvocationList(), this, Convert.ToBoolean(cpRequestPacket.Words[1]));
                    }
                }
            }
        }

        protected virtual void DispatchVarsHudPointsTrackerResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.HudPointsTracker != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        FrostbiteConnection.RaiseEvent(this.HudPointsTracker.GetInvocationList(), this, Convert.ToBoolean(cpRecievedPacket.Words[1]));
                    } else if (cpRequestPacket.Words.Count >= 2) {
                        FrostbiteConnection.RaiseEvent(this.HudPointsTracker.GetInvocationList(), this, Convert.ToBoolean(cpRequestPacket.Words[1]));
                    }
                }
            }
        }

        protected virtual void DispatchVarsHudUnlocksResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.HudUnlocks != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        FrostbiteConnection.RaiseEvent(this.HudUnlocks.GetInvocationList(), this, Convert.ToBoolean(cpRecievedPacket.Words[1]));
                    } else if (cpRequestPacket.Words.Count >= 2) {
                        FrostbiteConnection.RaiseEvent(this.HudUnlocks.GetInvocationList(), this, Convert.ToBoolean(cpRequestPacket.Words[1]));
                    }
                }
            }
        }

        protected virtual void DispatchVarsPlaylistResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.Playlist != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        FrostbiteConnection.RaiseEvent(this.Playlist.GetInvocationList(), this, Convert.ToString(cpRecievedPacket.Words[1]));
                    } else if (cpRequestPacket.Words.Count >= 2) {
                        FrostbiteConnection.RaiseEvent(this.Playlist.GetInvocationList(), this, Convert.ToString(cpRequestPacket.Words[1]));
                    }
                }
            }
        }

        #endregion
        #endregion

        #region ReservedSlotsList.AggressiveJoin
        protected virtual void DispatchReservedSlotsAggressiveJoinResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.ReservedSlotsListAggressiveJoin != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        FrostbiteConnection.RaiseEvent(this.ReservedSlotsListAggressiveJoin.GetInvocationList(), this, Convert.ToBoolean(cpRecievedPacket.Words[1]));
                    } else if (cpRequestPacket.Words.Count >= 2) {
                        FrostbiteConnection.RaiseEvent(this.ReservedSlotsListAggressiveJoin.GetInvocationList(), this, Convert.ToBoolean(cpRequestPacket.Words[1]));
                    }
                }
            }
        }
        #endregion

        #region RoundLockdownCountdown & RoundWarmupTimeout

        protected virtual void DispatchVarsRoundLockdownCountdownResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket)
        {
            if (cpRequestPacket.Words.Count >= 1)
            {
                if (this.RoundLockdownCountdown != null)
                {
                    if (cpRecievedPacket.Words.Count == 2)
                    {
                        FrostbiteConnection.RaiseEvent(this.RoundLockdownCountdown.GetInvocationList(), this, Convert.ToInt32(cpRecievedPacket.Words[1]));
                    }
                    else if (cpRequestPacket.Words.Count >= 2)
                    {
                        FrostbiteConnection.RaiseEvent(this.RoundLockdownCountdown.GetInvocationList(), this, Convert.ToInt32(cpRequestPacket.Words[1]));
                    }
                }
            }
        }

        protected virtual void DispatchVarsRoundWarmupTimeoutResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket)
        {
            if (cpRequestPacket.Words.Count >= 1)
            {
                if (this.RoundWarmupTimeout != null)
                {
                    if (cpRecievedPacket.Words.Count == 2)
                    {
                        FrostbiteConnection.RaiseEvent(this.RoundWarmupTimeout.GetInvocationList(), this, Convert.ToInt32(cpRecievedPacket.Words[1]));
                    }
                    else if (cpRequestPacket.Words.Count >= 2)
                    {
                        FrostbiteConnection.RaiseEvent(this.RoundWarmupTimeout.GetInvocationList(), this, Convert.ToInt32(cpRequestPacket.Words[1]));
                    }
                }
            }
        }
        #endregion

        #region PremiumStatus
        protected virtual void DispatchVarsPremiumStatusResponse(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket)
        {
            if (cpRequestPacket.Words.Count >= 1) {
                if (this.PremiumStatus != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        FrostbiteConnection.RaiseEvent(this.PremiumStatus.GetInvocationList(), this, Convert.ToBoolean(cpRecievedPacket.Words[1]));
                    }
                    else if (cpRequestPacket.Words.Count >= 2) {
                        FrostbiteConnection.RaiseEvent(this.PremiumStatus.GetInvocationList(), this, Convert.ToBoolean(cpRequestPacket.Words[1]));
                    }
                }
            }
        }
        #endregion

        #endregion

    }
}
