using ProjectOptimizationApp.Models;
using System.Collections.Generic;

namespace ProjectOptimizationApp
{
    class Globals
    {
        public int totalCost = 0;
        public List<Activity> currentNetworkState = new List<Activity>();

        private Globals() { }
        private static Globals _state;

        public static Globals GetState()
        {
            if (_state == null)
            {
                _state = new Globals();
            }
            return _state;
        }
    }
}
