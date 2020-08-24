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

            Console.WriteLine("Podaj wysokość spodziewanych zysków: ");
            globals.expectedEarnings = Convert.ToInt32(Console.ReadLine());

            activitiesData = ExcelReader.Read("dane.xlsx");

            var totalCost = activitiesData.Sum(x => x.Cost * x.Duration);

            globals.basicTotalCost = totalCost;
            globals.currentTotalCost = totalCost;

            Console.WriteLine("Pierwotny koszt projektu: {0}", globals.basicTotalCost);
            
            activitiesData = _service.SetSuccessors(activitiesData);
            activitiesData = _service.CalculateAhead(activitiesData);
            activitiesData = _service.CalculateBackwards(activitiesData);
            _service.FindCriticalPath(activitiesData);

            Console.WriteLine("Ścieżka krytyczna: ");
            foreach(var actId in globals.criticalPath)
            {
                Console.Write("{0} ", actId);
            }

            activitiesData = _service.CalculateAverageCostGradient(activitiesData);

            foreach (var item in activitiesData)
            {
                var copyItem = Clone(item);
                globals.currentNetworkState.Add(copyItem);
            }

            _service.Optimize(activitiesData);
            Console.ReadKey();
        }

        private static T Clone<T>(T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}
