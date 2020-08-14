using Newtonsoft.Json;
using ProjectOptimizationApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectOptimizationApp
{
    class Program
    {
        static void Main()
        {
            IService _service = new Service();
            Globals globals = Globals.GetState();
            var activitiesData = new List<Activity>();

            activitiesData = ExcelReader.Read("test.xlsx");

            globals.totalCost = activitiesData.Sum(x => x.Cost);
            Console.WriteLine("TOTAL COST: {0}", globals.totalCost);
            
            activitiesData = _service.SetSuccessors(activitiesData);
            activitiesData = _service.CalculateAhead(activitiesData);
            activitiesData = _service.CalculateBackwards(activitiesData);
            _service.FindCriticalPath(activitiesData);          
            activitiesData = _service.CalculateAverageCostGradient(activitiesData);

            foreach (var item in activitiesData)
            {
                var copyItem = Clone(item);
                globals.currentNetworkState.Add(copyItem);
            }

            _service.Optimize(activitiesData);
        }

        private static T Clone<T>(T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}
