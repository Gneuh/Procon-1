using System;
using System.Collections.Generic;
using System.Text;

namespace PRoCon.Core.Remote.Layer {
    public class BF3LayerClient : FrostbiteLayerClient {

        public BF3LayerClient(FrostbiteLayerConnection connection) : base(connection) {

            this.m_requestDelegates.Add("admin.eventsEnabled", this.DispatchEventsEnabledRequest);

            // vars.idleTimeout is already included in FrostbiteLayerClient
            //this.m_requestDelegates.Add("vars.idleTimeout", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.idleBanRounds", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.maxPlayers", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.3pCam", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.vehicleSpawnAllowed", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.vehicleSpawnDelay", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.bulletDamage", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.nameTag", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.regenerateHealth", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.roundRestartPlayerCount", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.onlySquadLeaderSpawn", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.unlockMode", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.gunMasterWeaponsPreset", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.soldierHealth", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.hud", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.playerManDownTime", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.roundStartPlayerCount", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.playerRespawnTime", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.gameModeCounter", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.ctfRoundTimeModifier", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.roundLockdownCountdown", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.roundWarmupTimeout", this.DispatchVarsRequest);

            this.m_requestDelegates.Add("vars.killCam", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.miniMap", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.crossHair", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.3dSpotting", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.miniMapSpotting", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.thirdPersonVehicleCameras", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.autoBalance", this.DispatchVarsRequest);

            this.m_requestDelegates.Add("reservedSlotsList.configFile", this.DispatchAlterReservedSlotsListRequest);
            this.m_requestDelegates.Add("reservedSlotsList.load", this.DispatchAlterReservedSlotsListRequest);
            this.m_requestDelegates.Add("reservedSlotsList.save", this.DispatchAlterReservedSlotsListRequest);
            this.m_requestDelegates.Add("reservedSlotsList.add", this.DispatchAlterReservedSlotsListRequest);
            this.m_requestDelegates.Add("reservedSlotsList.remove", this.DispatchAlterReservedSlotsListRequest);
            this.m_requestDelegates.Add("reservedSlotsList.clear", this.DispatchAlterReservedSlotsListRequest);
            this.m_requestDelegates.Add("reservedSlotsList.list", this.DispatchSecureSafeListedRequest);
            this.m_requestDelegates.Add("reservedSlotsList.aggressiveJoin", this.DispatchVarsRequest);

            this.m_requestDelegates.Add("currentLevel", this.DispatchSecureSafeListedRequest);

            this.m_requestDelegates.Add("mapList.add", this.DispatchAlterMaplistRequest);

            this.m_requestDelegates.Add("mapList.runNextRound", this.DispatchUseMapFunctionRequest);
            this.m_requestDelegates.Add("mapList.restartRound", this.DispatchUseMapFunctionRequest);
            this.m_requestDelegates.Add("mapList.endRound", this.DispatchUseMapFunctionRequest);
            this.m_requestDelegates.Add("mapList.setNextMapIndex", this.DispatchUseMapFunctionRequest);
            this.m_requestDelegates.Add("mapList.getMapIndices", this.DispatchSecureSafeListedRequest);
            this.m_requestDelegates.Add("mapList.getRounds", this.DispatchUseMapFunctionRequest);

            this.m_requestDelegates.Add("vars.serverMessage", this.DispatchVarsRequest);
            this.m_requestDelegates.Add("vars.premiumStatus", this.DispatchVarsRequest);

            this.m_requestDelegates.Add("player.idleDuration", this.DispatchSecureSafeListedRequest);
            this.m_requestDelegates.Add("player.isAlive", this.DispatchSecureSafeListedRequest);
            this.m_requestDelegates.Add("player.ping", this.DispatchSecureSafeListedRequest);
            this.m_requestDelegates.Add("squad.leader", this.DispatchSquadLeaderRequest);
            this.m_requestDelegates.Add("squad.listActive", this.DispatchSecureSafeListedRequest);
            this.m_requestDelegates.Add("squad.listPlayers", this.DispatchSecureSafeListedRequest);
            this.m_requestDelegates.Add("squad.private", this.DispatchSquadIsPrivateRequest);
        }

    }
}
