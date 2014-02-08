using System;
using System.ComponentModel;

namespace PRoCon {
    public static class ControlHelpers {
        private readonly static object[] Empty = new object[0];

        public static void InvokeIfRequired(this ISynchronizeInvoke control, Action action) {
            if (control.InvokeRequired) {
                control.Invoke(action, Empty);
            }
            else {
                action();
            }
        }
    }
}
