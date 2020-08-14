using System;
using System.Collections.Generic;

namespace ProjectOptimizationApp.Models
{
    [Serializable]
    public class Activity
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public int TerminalDuration { get; set; }
        public int Cost { get; set; }
        public int TerminalCost { get; set; }
        public int AverageCostGradient { get; set; }
        public int EarliestStartTime { get; set; }
        public int LatestStartTime { get; set; }
        public int EarliestEndTime { get; set; }
        public int LatestEndTime { get; set; }
        public List<int> Predecessors { get; set; }
        public List<int> Successors { get; set; } = new List<int>();
        public bool IsCriticalPath { get; set; }
    }
}
