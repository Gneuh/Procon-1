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
