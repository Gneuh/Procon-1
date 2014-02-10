using System;
using System.Collections.Generic;
using System.Linq;
using PRoCon.Core;

namespace PRoCon.Controls.Data {
    public class BansSource : ISource {
        private string _filter;
        private int _skip;
        private int _take;

        /// <summary>
        /// The items stored for this source
        /// </summary>
        public List<CBanInfo> Items { get; set; }

        /// <summary>
        /// When the items were last set. We use this to ignore full refreshes if one was done
        /// in the last minute. Prevents redrawing information we already know about.
        /// </summary>
        protected DateTime ItemsAge { get; set; }

        /// <summary>
        /// The items that have been filtered
        /// </summary>
        public List<CBanInfo> Filtered { get; set; }

        public int Count { get { return this.Filtered.Count(); } }

        public int Skip {
            get { return _skip; }
            set {
                if (_skip != value) {
                    _skip = value;
                    this.OnChange();
                }
            }
        }

        public int Take {
            get { return _take; }
            set {
                if (_take != value) {
                    _take = value;
                    this.OnChange();
                }
            }
        }

        public String Filter {
            get { return this._filter; }
            set {
                if (_filter != value) {
                    _filter = value;
                    this.RefreshFilter();
                    this.OnChange();
                }
            }
        }

        public event Action Changed;

        /// <summary>
        /// Fires off the change event
        /// </summary>
        protected void OnChange() {
            var handler = this.Changed;
            if (handler != null) {
                handler();
            }
        }

        /// <summary>
        /// Updates the filtered content with items that exist in Items and the current filter
        /// </summary>
        protected void RefreshFilter() {
            if (String.IsNullOrEmpty(this.Filter) == false) {
                var filter = this.Filter.ToLower();

                this.Filtered = this.Items.Where(item =>
                    (item.SoldierName != null && item.SoldierName.ToLower().Contains(filter)) ||
                    (item.IpAddress != null && item.IpAddress.ToLower().Contains(filter)) ||
                    (item.Guid != null && item.Guid.ToLower().Contains(filter)) ||
                    (item.Reason != null && item.Reason.ToLower().Contains(filter))
                ).ToList();
            }
            else {
                this.Filtered = this.Items;
            }
        }

        public void Set<T>(IEnumerable<T> items) {
            if (typeof(T) != typeof(CBanInfo)) throw new InvalidCastException();

            // If we have nothing yet or the items we do have are expired.
            if (this.Items == null || this.Items.Count == 0 || this.ItemsAge > DateTime.Now.AddMinutes(-1)) {
                this.Items = items.Cast<CBanInfo>().ToList();
                this.ItemsAge = DateTime.Now;

                this.RefreshFilter();
                this.OnChange();
            }
        }

        IEnumerable<T> ISource.Fetch<T>() {
            if (typeof(T) != typeof(CBanInfo)) throw new InvalidCastException();

            return this.Filtered.Skip(this.Skip).Take(this.Take).Cast<T>();
        }
    }
}
