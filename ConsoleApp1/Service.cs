using ProjectOptimizationApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectOptimizationApp
{
    public class Service : IService
    {
        public List<Activity> SetSuccessors(List<Activity> list)
        {
            foreach (var act in list)
            {
                var successors = list.FindAll(x => x.Predecessors.Contains(act.Id));

                foreach (var successor in successors)
                {
                    act.Successors.Add(successor.Id);
                }
            }

            return list;
        }

        public List<Activity> CalculateAhead(List<Activity> list)
        {
            list[0].EarliestEndTime = list[0].EarliestStartTime + list[0].Duration;

            for (int i = 1; i < list.Count; i++)
            {
                foreach (int actId in list[i].Predecessors)
                {
                    Activity act = list.Find(x => x.Id == actId);

                    if (list[i].EarliestStartTime < act.EarliestEndTime)
                    {
                        list[i].EarliestStartTime = act.EarliestEndTime;
                    }
                }

                list[i].EarliestEndTime = list[i].EarliestStartTime + list[i].Duration;
            }

            return list;
        }

        public List<Activity> CalculateBackwards(List<Activity> list)
        {
            var noOfElements = list.Count();

            list[noOfElements - 1].LatestEndTime = list[noOfElements - 1].EarliestEndTime;
            list[noOfElements - 1].LatestStartTime = list[noOfElements - 1].LatestEndTime - list[noOfElements - 1].Duration;

            for (int i = noOfElements - 2; i >= 0; i--)
            {
                foreach (int actId in list[i].Successors)
                {
                    Activity act = list.Find(x => x.Id == actId);

                    if (list[i].LatestEndTime == 0)
                    {
                        list[i].LatestEndTime = act.LatestStartTime;
                    }
                    else
                    {
                        if (list[i].LatestEndTime > act.LatestStartTime)
                            list[i].LatestEndTime = act.LatestStartTime;
                    }
                }

                list[i].LatestStartTime = list[i].LatestEndTime - list[i].Duration;
            }

            return list;
        }

        public void FindCriticalPath(List<Activity> list)
        {            
            foreach (Activity act in list)
            {
                if ((act.EarliestEndTime - act.LatestEndTime == 0) && (act.EarliestStartTime - act.LatestStartTime == 0))
                {
                    act.IsCriticalPath = true;
                    Console.WriteLine("{0} ", act.Id);
                }
            }

            Console.WriteLine("Total Duration: {0}", list[list.Count - 1].EarliestEndTime);
        }

        public List<Activity> CalculateAverageCostGradient(List<Activity> list)
        {
            foreach (var act in list)
            {
                act.AverageCostGradient = (act.TerminalCost - act.Cost) / (act.Duration - act.TerminalDuration);
            }

            return list;
        }

        public void Optimize(List<Activity> activitiesData)
        {
            Globals globals = Globals.GetState();

            foreach (var workspaceItem in globals.currentNetworkState.OrderBy(x => x.AverageCostGradient).ToList())
            {
                if (workspaceItem.IsCriticalPath)
                {
                    ClearCalculations(globals.currentNetworkState);

                    workspaceItem.Duration = workspaceItem.TerminalDuration;

                    globals.currentNetworkState = CalculateAhead(globals.currentNetworkState);
                    globals.currentNetworkState = CalculateBackwards(globals.currentNetworkState);
                    FindCriticalPath(globals.currentNetworkState);

                    var originalAct = activitiesData.Where(x => x.Id == workspaceItem.Id).FirstOrDefault();

                    globals.totalCost = globals.totalCost + (originalAct.Duration - originalAct.TerminalDuration) * originalAct.AverageCostGradient;

                    Console.WriteLine("Total cost: {0}", globals.totalCost);
                }
            }
        }

        private void ClearCalculations(List<Activity> list)
        {
            foreach (var activity in list)
            {
                activity.EarliestStartTime = 0;
                activity.EarliestEndTime = 0;
                activity.LatestStartTime = 0;
                activity.LatestEndTime = 0;
            }
        }
    }
}
