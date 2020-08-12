using ProjectOptimizationApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectOptimizationApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var activities = new List<Activity>();
            activities = ExcelReader.Read("test.xlsx");
            activities = SetSuccessors(activities);
            activities = CalculateAhead(activities);
            activities = CalculateBackwards(activities);
            FindCriticalPath(activities);
        }

        private static List<Activity> SetSuccessors(List<Activity> list)
        {
            foreach(var act in list)
            {
                var successors = list.FindAll(x => x.Predecessors.Contains(act.Id));
                
                foreach(var successor in successors)
                {
                    act.Successors.Add(successor.Id);
                }
            }

            return list;
        }

        private static List<Activity> CalculateAhead(List<Activity> list)
        {
            list[0].EarliestEndTime = list[0].EarliestStartTime + list[0].Duration;

            for(int i = 1; i < list.Count; i++)
            {
                foreach(int actId in list[i].Predecessors)
                {
                    Activity act = list.Find(x => x.Id == actId);

                    if(list[i].EarliestStartTime < act.EarliestEndTime)
                    {
                        list[i].EarliestStartTime = act.EarliestEndTime;
                    }
                }

                list[i].EarliestEndTime = list[i].EarliestStartTime + list[i].Duration;
            }

            return list;
        }

        private static List<Activity> CalculateBackwards(List<Activity> list)
        {
            var noOfElements = list.Count();

            list[noOfElements - 1].LatestEndTime = list[noOfElements - 1].EarliestEndTime;
            list[noOfElements - 1].LatestStartTime = list[noOfElements - 1].LatestEndTime - list[noOfElements - 1].Duration;

            for (int i = noOfElements - 2 ; i >= 0; i--)
            {
                foreach(int actId in list[i].Successors)
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

        private static void FindCriticalPath(List<Activity> list)
        {
            foreach(Activity act in list)
            {
                if((act.EarliestEndTime - act.LatestEndTime ==0) && (act.EarliestStartTime - act.LatestStartTime == 0))
                {
                    act.IsCriticalPath = true;                   
                }                
            }

            Console.WriteLine("Total Duration: {0}", list[list.Count - 1].EarliestEndTime);
        }
    }
}
