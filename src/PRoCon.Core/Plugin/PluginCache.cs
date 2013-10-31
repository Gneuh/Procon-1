using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PRoCon.Core.Plugin {
    [Serializable]
    public class PluginCache {

        /// <summary>
        /// The list of plugins being cached
        /// </summary>
        public List<PluginCacheEntry> Entries { get; set; } 

        public PluginCache() {
            this.Entries = new List<PluginCacheEntry>();
        }

        public bool IsModified(String className, String hash) {
            bool isModified = true;

            PluginCacheEntry pluginEntry = this.Entries.FirstOrDefault(entry => entry.ClassName == className);

            if (pluginEntry != null) {
                isModified = String.Compare(pluginEntry.Hash, hash, StringComparison.OrdinalIgnoreCase) != 0;
            }

            return isModified;
        }

        public void Cache(PluginCacheEntry entry) {
            PluginCacheEntry pluginEntry = null;

            do {
                pluginEntry = this.Entries.FirstOrDefault(e => e.ClassName == entry.ClassName);

                if (pluginEntry != null) {
                    this.Entries.Remove(pluginEntry);
                }

            } while (pluginEntry != null);
            
            this.Entries.Add(entry);
        }
    }
}
