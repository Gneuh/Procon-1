// Copyright 2010 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of PRoCon Frostbite.
// 
// PRoCon Frostbite is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// PRoCon Frostbite is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with PRoCon Frostbite.  If not, see <http://www.gnu.org/licenses/>.

using System.Collections.ObjectModel;
using System.Linq;

namespace PRoCon.Core.Remote.Layer {
    using Core.Accounts;
    using Core.Remote;
    public class LayerClientDictionary : KeyedCollection<string, PRoConLayerClient> {

        public delegate void LayerClientHandler(Account item);
        public event LayerClientHandler LayerClientConnected;
        public event LayerClientHandler LayerClientAltered;
        public event LayerClientHandler LayerClientDisconnected;

        protected override string GetKeyForItem(PRoConLayerClient item) {
            return item.IPPort;
        }

        protected override void InsertItem(int index, PRoConLayerClient item) {
            if (this.LayerClientConnected != null) {
                FrostbiteConnection.RaiseEvent(this.LayerClientConnected.GetInvocationList(), item);
            }

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index) {

            if (this.LayerClientDisconnected != null) {
                FrostbiteConnection.RaiseEvent(this.LayerClientDisconnected.GetInvocationList(), this[index]);
            }

            base.RemoveItem(index);
        }

        protected override void SetItem(int index, PRoConLayerClient item) {
            if (this.LayerClientAltered != null) {
                FrostbiteConnection.RaiseEvent(this.LayerClientAltered.GetInvocationList(), item);
            }

            base.SetItem(index, item);
        }

        public bool IsUidUnique(string strProconEventsUid) {
            return this.All(plcUidCheck => plcUidCheck.ProconEventsUid == null || System.String.Compare(plcUidCheck.ProconEventsUid, strProconEventsUid, System.StringComparison.Ordinal) != 0);
        }
    }
}
