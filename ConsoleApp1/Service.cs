using ProjectOptimizationApp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
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
                        {
                            list[i].LatestEndTime = act.LatestStartTime;
                        }                         
                    }
                }

                list[i].LatestStartTime = list[i].LatestEndTime - list[i].Duration;
            }

            return list;
        }

        public void FindCriticalPath(List<Activity> list)
        {
            Globals globals = Globals.GetState();

            foreach (Activity act in list)
            {
                if ((act.EarliestEndTime - act.LatestEndTime == 0) && (act.EarliestStartTime - act.LatestStartTime == 0))
                {
                    act.IsCriticalPath = true;
                    globals.criticalPath.Add(act.Id);                   
                }
            }

            int totalDuration = list[list.Count - 1].EarliestEndTime;

            if (globals.basicTotalDuration == 0)
            {
                globals.basicTotalDuration = totalDuration;
            }

            globals.currentTotalDuration = totalDuration;

            Console.WriteLine("Pierwotny czas trwania projektu: {0}", globals.basicTotalDuration);
            Console.WriteLine("Obecny czas trwania projektu: {0}", globals.currentTotalDuration);
        }

        public List<Activity> CalculateAverageCostGradient(List<Activity> list)
        {
            foreach (var act in list)
            {
                if(act.Duration - act.TerminalDuration > 0)
                {
                    act.AverageCostGradient = (act.TerminalCost - act.Cost) / (act.Duration - act.TerminalDuration);
                }
                else
                {
                    act.AverageCostGradient = -1;
                }
            }

            return list;
        }

        public void Optimize(List<Activity> activitiesData)
        {
            Globals globals = Globals.GetState();

            foreach (var workspaceItem in globals.currentNetworkState.OrderBy(x => x.AverageCostGradient).ToList())
            {
                if (workspaceItem.IsCriticalPath && workspaceItem.AverageCostGradient != -1)
                {
                    Console.WriteLine("\n");
                    Console.WriteLine("Optymalizacja zadania o id = {0}", workspaceItem.Id);
                    Console.WriteLine("Gradient kosztu: {0}", workspaceItem.AverageCostGradient);

                    ClearCalculations(globals.currentNetworkState);

                    workspaceItem.Duration = workspaceItem.TerminalDuration;

                    globals.currentNetworkState = CalculateAhead(globals.currentNetworkState);
                    globals.currentNetworkState = CalculateBackwards(globals.currentNetworkState);
                    FindCriticalPath(globals.currentNetworkState);

                    var originalAct = activitiesData.Where(x => x.Id == workspaceItem.Id).FirstOrDefault();

                    globals.currentTotalCost += (originalAct.Duration - originalAct.TerminalDuration) * originalAct.AverageCostGradient;

                    Console.WriteLine("Obecny koszt całkowity projektu: {0}", globals.currentTotalCost);

                    CalculateEffectiveness();
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

        private void CalculateEffectiveness()
        {
            Globals _globals = Globals.GetState();

            var effectiveness = _globals.expectedEarnings / _globals.currentTotalCost;

            double performance = _globals.basicTotalDuration / _globals.currentTotalDuration;

            var costChange = (_globals.currentTotalCost / _globals.basicTotalCost) - 1;

            Console.WriteLine("Efektywność projektu (stosunek spodziewanych zysków do kosztów projektu) : {0}", Math.Round(effectiveness, 2));
            Console.WriteLine("Wydajność projektu (stosunek pierwotnego czasu trwania projektu do obecnego czasu trwania projektu): {0}",
                                                                                                                    Math.Round(performance, 2));
            Console.WriteLine("Zmiana kosztu projektu względem pierwotnego kosztu: {0}", costChange.ToString("P", CultureInfo.InvariantCulture));
            Console.WriteLine("\n");
        }
    }
}
