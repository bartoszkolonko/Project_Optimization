using ProjectOptimizationApp.Models;
using System.Collections.Generic;

namespace ProjectOptimizationApp
{
    public interface IService
    {
        List<Activity> SetSuccessors(List<Activity> list);
        List<Activity> CalculateAhead(List<Activity> list);
        List<Activity> CalculateBackwards(List<Activity> list);
        void FindCriticalPath(List<Activity> list);
        List<Activity> CalculateAverageCostGradient(List<Activity> list);
        void Optimize(List<Activity> activitiesData);
    }
}
