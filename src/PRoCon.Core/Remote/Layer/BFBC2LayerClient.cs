using System;
using System.Collections.Generic;
using System.Text;

namespace PRoCon.Core.Remote.Layer {
    public class BFBC2LayerClient : FrostbiteLayerClient {

        public BFBC2LayerClient(FrostbiteLayerConnection connection)
            : base(connection) {

            this.RequestDelegates.Add("vars.killCam", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.miniMap", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.crossHair", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.3dSpotting", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.miniMapSpotting", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.thirdPersonVehicleCameras", this.DispatchVarsRequest);
            this.RequestDelegates.Add("vars.teamBalance", this.DispatchVarsRequest);

            this.RequestDelegates.Add("reservedSlots.configFile", this.DispatchAlterReservedSlotsListRequest);
            this.RequestDelegates.Add("reservedSlots.load", this.DispatchAlterReservedSlotsListRequest);
            this.RequestDelegates.Add("reservedSlots.save", this.DispatchAlterReservedSlotsListRequest);
            this.RequestDelegates.Add("reservedSlots.addPlayer", this.DispatchAlterReservedSlotsListRequest);
            this.RequestDelegates.Add("reservedSlots.removePlayer", this.DispatchAlterReservedSlotsListRequest);
            this.RequestDelegates.Add("reservedSlots.clear", this.DispatchAlterReservedSlotsListRequest);
            this.RequestDelegates.Add("reservedSlots.list", this.DispatchSecureSafeListedRequest);
        }
    }
}
