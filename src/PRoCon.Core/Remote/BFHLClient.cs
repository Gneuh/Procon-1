using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRoCon.Core.Remote {
    public class BFHLClient : BF4Client {
        public BFHLClient(FrostbiteConnection connection) : base(connection) {
            ResponseDelegates.Add("vars.roundStartReadyPlayersNeeded", DispatchVarsRoundStartReadyPlayersNeeded);
        }

        public override string GameType {
            get { return "BFHL"; }
        }

        public override void FetchStartupVariables() {
            base.FetchStartupVariables();

            this.SendGetVarsRoundRestartPlayerCountPacket();
        }

        #region events

        public override event LimitHandler RoundStartReadyPlayersNeeded;

        #endregion

        #region Vars

        protected virtual void DispatchVarsRoundStartReadyPlayersNeeded(FrostbiteConnection sender, Packet cpRecievedPacket, Packet cpRequestPacket) {
            if (cpRequestPacket.Words.Count >= 1) {
                var handler = this.RoundStartReadyPlayersNeeded;

                if (handler != null) {
                    if (cpRecievedPacket.Words.Count == 2) {
                        handler(this, Convert.ToInt32(cpRecievedPacket.Words[1]));
                    }
                    else if (cpRequestPacket.Words.Count >= 2) {
                        handler(this, Convert.ToInt32(cpRequestPacket.Words[1]));
                    }
                }
            }
        }

        #endregion
    }
}
