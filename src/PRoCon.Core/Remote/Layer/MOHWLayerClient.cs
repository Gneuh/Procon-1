using System;
using System.Collections.Generic;
using System.Text;

namespace PRoCon.Core.Remote.Layer {
    public class MOHWLayerClient : FrostbiteLayerClient {

        public MOHWLayerClient(FrostbiteLayerConnection connection) : base(connection) {

            this.m_requestDelegates.Add("admin.eventsEnabled", this.DispatchEventsEnabledRequest);

            #region FrostbiteLayerClient delegates
            // vars.bannerUrl -> FrostbiteLayerClient
            // vars.friendlyFire -> FrostbiteLayerClient
            // vars.gamePassword -> FrostbiteLayerClient
            // vars.idleTimeout -> FrostbiteLayerClient
            // vars.ranked -> FrostbiteLayerClient
            // vars.serverDescription
            // vars.serverName
            // vars.teamKill*
            #endregion

            #region MoHW R-3 disabled
            // MoHW R-3 this.m_requestDelegates.Add("reservedSlotsList.aggressiveJoin", this.DispatchVarsRequest);

            // MoHW R-3 this.m_requestDelegates.Add("vars.3dSpotting", this.DispatchVarsRequest);
            // MoHW R-3 this.m_requestDelegates.Add("vars.hud", this.DispatchVarsRequest);
            // MoHW R-3 this.m_requestDelegates.Add("vars.miniMap", this.DispatchVarsRequest);
            // this.m_requestDelegates.Add("vars.miniMapSpotting", this.DispatchVarsRequest);
            // MoHW R-3 this.m_requestDelegates.Add("vars.nameTag", this.DispatchVarsRequest);
            // MoHW R-3 this.m_requestDelegates.Add("vars.onlySquadLeaderSpawn", this.DispatchVarsRequest);
            // this.m_requestDelegates.Add("vars.thirdPersonVehicleCameras", this.DispatchVarsRequest);
            // MOHW general this.m_requestDelegates.Add("vars.premiumStatus", this.DispatchVarsRequest);
            // MoHW R-3 this.m_requestDelegates.Add("vars.roundLockdownCountdown", this.DispatchVarsRequest);
            // MoHW R-3 this.m_requestDelegates.Add("vars.roundWarmupTimeout", this.DispatchVarsRequest);
            // MoHW R-3 this.m_requestDelegates.Add("vars.unlockMode", this.DispatchVarsRequest);
            // MoHW R-3 this.m_requestDelegates.Add("vars.vehicleSpawnAllowed", this.DispatchVarsRequest);
            // MoHW R-3 this.m_requestDelegates.Add("vars.vehicleSpawnDelay", this.DispatchVarsRequest);
            #endregion

            this.m_requestDelegates.Add("vars.3pCam", this.DispatchVarsRequest);
            // R-5 this.m_requestDelegates.Add("vars.allUnlocksUnlocked", this.DispatchVarsRequest); // MoHW
            this.m_requestDelegates.Add("vars.autoBalance", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.buddyOutline", this.DispatchVarsRequest); // MoHW
            this.m_requestDelegates.Add("vars.bulletDamage", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.gameModeCounter", this.DispatchVarsRequest);

            this.m_requestDelegates.Add("vars.hudBuddyInfo", this.DispatchVarsRequest); // MoHW
            this.m_requestDelegates.Add("vars.hudClassAbility", this.DispatchVarsRequest); // MoHW
            this.m_requestDelegates.Add("vars.hudCrosshair", this.DispatchVarsRequest); // MoHW
            this.m_requestDelegates.Add("vars.hudEnemyTag", this.DispatchVarsRequest); // MoHW
            this.m_requestDelegates.Add("vars.hudExplosiveIcons", this.DispatchVarsRequest); // MoHW
            this.m_requestDelegates.Add("vars.hudGameMode", this.DispatchVarsRequest); // MoHW
            this.m_requestDelegates.Add("vars.hudHealthAmmo", this.DispatchVarsRequest); // MoHW
            this.m_requestDelegates.Add("vars.hudMinimap", this.DispatchVarsRequest); // MoHW
            this.m_requestDelegates.Add("vars.hudObiturary", this.DispatchVarsRequest); // MoHW
            this.m_requestDelegates.Add("vars.hudPointsTracker", this.DispatchVarsRequest); // MoHW
            this.m_requestDelegates.Add("vars.hudUnlocks", this.DispatchVarsRequest); // MoHW
            
            this.m_requestDelegates.Add("vars.idleBanRounds", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.killCam", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.maxPlayers", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.playlist", this.DispatchAlterMaplistRequest); // MoHW
            this.m_requestDelegates.Add("vars.playerManDownTime", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.playerRespawnTime", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.regenerateHealth", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.roundRestartPlayerCount", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.roundStartPlayerCount", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.soldierHealth", this.DispatchVarsRequest);
            
            this.m_requestDelegates.Add("reservedSlotsList.configFile", this.DispatchAlterReservedSlotsListRequest);
            this.m_requestDelegates.Add("reservedSlotsList.load", this.DispatchAlterReservedSlotsListRequest);
            this.m_requestDelegates.Add("reservedSlotsList.save", this.DispatchAlterReservedSlotsListRequest);
            this.m_requestDelegates.Add("reservedSlotsList.add", this.DispatchAlterReservedSlotsListRequest);
            this.m_requestDelegates.Add("reservedSlotsList.remove", this.DispatchAlterReservedSlotsListRequest);
            this.m_requestDelegates.Add("reservedSlotsList.clear", this.DispatchAlterReservedSlotsListRequest);
            this.m_requestDelegates.Add("reservedSlotsList.list", this.DispatchSecureSafeListedRequest);
            
            this.m_requestDelegates.Add("currentLevel", this.DispatchSecureSafeListedRequest);

            this.m_requestDelegates.Add("mapList.add", this.DispatchAlterMaplistRequest);

            this.m_requestDelegates.Add("mapList.runNextRound", this.DispatchUseMapFunctionRequest);
            this.m_requestDelegates.Add("mapList.restartRound", this.DispatchUseMapFunctionRequest);
            this.m_requestDelegates.Add("mapList.endRound", this.DispatchUseMapFunctionRequest);
            this.m_requestDelegates.Add("mapList.setNextMapIndex", this.DispatchUseMapFunctionRequest);
            this.m_requestDelegates.Add("mapList.getMapIndices", this.DispatchSecureSafeListedRequest);
            this.m_requestDelegates.Add("mapList.getRounds", this.DispatchUseMapFunctionRequest);

            this.m_requestDelegates.Add("vars.serverMessage", this.DispatchVarsRequest);
        }
    }
}
