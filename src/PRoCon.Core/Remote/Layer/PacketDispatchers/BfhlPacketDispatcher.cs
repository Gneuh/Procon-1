﻿namespace PRoCon.Core.Remote.Layer.PacketDispatchers {
    public class BfhlPacketDispatcher : LayerPacketDispatcher {
        public BfhlPacketDispatcher(ILayerConnection connection) : base(connection) {
            this.RequestDelegates.Add("admin.eventsEnabled", this.DispatchEventsEnabledRequest);

            // vars.idleTimeout is already included in FrostbiteLayerClient
            //this.m_requestDelegates.Add("vars.idleTimeout", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.idleBanRounds", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.maxPlayers", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.3pCam", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.vehicleSpawnAllowed", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.vehicleSpawnDelay", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.bulletDamage", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.nameTag", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.regenerateHealth", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.roundRestartPlayerCount", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.onlySquadLeaderSpawn", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.unlockMode", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.preset", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.gunMasterWeaponsPreset", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.soldierHealth", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.hud", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.playerManDownTime", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.roundStartPlayerCount", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.playerRespawnTime", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.gameModeCounter", this.DispatchVarsRequest);
            
            this.RequestDelegates.Add("vars.roundLockdownCountdown", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.roundWarmupTimeout", this.DispatchVarsRequest);

            this.RequestDelegates.Add("vars.killCam", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.miniMap", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.crossHair", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.3dSpotting", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.miniMapSpotting", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.thirdPersonVehicleCameras", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.autoBalance", this.DispatchVarsRequest);

            this.RequestDelegates.Add("reservedSlotsList.configFile", this.DispatchAlterReservedSlotsListRequest);
            this.RequestDelegates.Add("reservedSlotsList.load", this.DispatchAlterReservedSlotsListRequest);
            this.RequestDelegates.Add("reservedSlotsList.save", this.DispatchAlterReservedSlotsListRequest);
            this.RequestDelegates.Add("reservedSlotsList.add", this.DispatchAlterReservedSlotsListRequest);
            this.RequestDelegates.Add("reservedSlotsList.remove", this.DispatchAlterReservedSlotsListRequest);
            this.RequestDelegates.Add("reservedSlotsList.clear", this.DispatchAlterReservedSlotsListRequest);
            this.RequestDelegates.Add("reservedSlotsList.list", this.DispatchSecureSafeListedRequest);
            this.RequestDelegates.Add("reservedSlotsList.aggressiveJoin", this.DispatchVarsRequest);

            this.RequestDelegates.Add("spectatorList.load", this.DispatchAlterReservedSlotsListRequest);
            this.RequestDelegates.Add("spectatorList.save", this.DispatchAlterReservedSlotsListRequest);
            this.RequestDelegates.Add("spectatorList.add", this.DispatchAlterReservedSlotsListRequest);
            this.RequestDelegates.Add("spectatorList.remove", this.DispatchAlterReservedSlotsListRequest);
            this.RequestDelegates.Add("spectatorList.clear", this.DispatchAlterReservedSlotsListRequest);
            this.RequestDelegates.Add("spectatorList.list", this.DispatchSecureSafeListedRequest);

            this.RequestDelegates.Add("gameAdmin.load", this.DispatchAlterReservedSlotsListRequest);
            this.RequestDelegates.Add("gameAdmin.save", this.DispatchAlterReservedSlotsListRequest);
            this.RequestDelegates.Add("gameAdmin.add", this.DispatchAlterReservedSlotsListRequest);
            this.RequestDelegates.Add("gameAdmin.remove", this.DispatchAlterReservedSlotsListRequest);
            this.RequestDelegates.Add("gameAdmin.clear", this.DispatchAlterReservedSlotsListRequest);
            this.RequestDelegates.Add("gameAdmin.list", this.DispatchSecureSafeListedRequest);

            this.RequestDelegates.Add("currentLevel", this.DispatchSecureSafeListedRequest);

            this.RequestDelegates.Add("mapList.add", this.DispatchAlterMaplistRequest);

            this.RequestDelegates.Add("mapList.runNextRound", this.DispatchUseMapFunctionRequest);
            this.RequestDelegates.Add("mapList.restartRound", this.DispatchUseMapFunctionRequest);
            this.RequestDelegates.Add("mapList.endRound", this.DispatchUseMapFunctionRequest);
            this.RequestDelegates.Add("mapList.setNextMapIndex", this.DispatchUseMapFunctionRequest);
            this.RequestDelegates.Add("mapList.getMapIndices", this.DispatchSecureSafeListedRequest);
            this.RequestDelegates.Add("mapList.getRounds", this.DispatchUseMapFunctionRequest);

            this.RequestDelegates.Add("vars.serverMessage", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.premiumStatus", this.DispatchVarsRequest);

            this.RequestDelegates.Add("player.idleDuration", this.DispatchSecureSafeListedRequest);
            this.RequestDelegates.Add("player.isAlive", this.DispatchSecureSafeListedRequest);
            this.RequestDelegates.Add("player.ping", this.DispatchSecureSafeListedRequest);
            this.RequestDelegates.Add("squad.leader", this.DispatchSquadLeaderRequest);
            this.RequestDelegates.Add("squad.listActive", this.DispatchSecureSafeListedRequest);
            this.RequestDelegates.Add("squad.listPlayers", this.DispatchSecureSafeListedRequest);
            this.RequestDelegates.Add("squad.private", this.DispatchSquadIsPrivateRequest);

            this.RequestDelegates.Add("punkBuster.isActive", this.DispatchVarsRequest);
            this.RequestDelegates.Add("punkBuster.activate", this.DispatchVarsRequest);
            this.RequestDelegates.Add("punkBuster.deactivate", this.DispatchVarsRequest);

            this.RequestDelegates.Add("fairFight.isActive", this.DispatchVarsRequest);
            this.RequestDelegates.Add("fairFight.activate", this.DispatchVarsRequest);
            this.RequestDelegates.Add("fairfight.deactivate", this.DispatchVarsRequest);
            
            this.RequestDelegates.Add("vars.maxSpectators", this.DispatchVarsRequest);
            
            this.RequestDelegates.Add("vars.hitIndicatorsEnabled", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.hacker", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.serverType", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.forceReloadWholeMags", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.alwaysAllowSpectators", this.DispatchVarsRequest);

            this.RequestDelegates.Add("vars.roundTimeLimit", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.ticketBleedRate", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.mpExperience", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.team1FactionOverride", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.team2FactionOverride", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.team3FactionOverride", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.team4FactionOverride", this.DispatchVarsRequest);
        }

    }
}
