using ProjectOptimizationApp.Models;
using System.Collections.Generic;

namespace ProjectOptimizationApp
{
    class Globals
    {
        public double expectedEarnings = 0.0;
        public double basicTotalCost = 0.0;
        public double currentTotalCost = 0.0;
        public double basicTotalDuration = 0.0;
        public double currentTotalDuration = 0.0;
        public List<int> criticalPath = new List<int>();
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
