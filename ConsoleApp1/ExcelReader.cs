using ExcelDataReader;
using ProjectOptimizationApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace ProjectOptimizationApp
{
    public static class ExcelReader
    {
        public static List<Activity> Read(string path)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            List<Activity> activities = new List<Activity>();

            using(var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                using (var excelReader = ExcelReaderFactory.CreateReader(stream))
                {

                    var conf = new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = true
                        }
                    };

                    var rawData = excelReader.AsDataSet(conf);

                    foreach (DataRow item in rawData.Tables[0].Rows)
                    {
                        var activity = new Activity();
                        activity.Id = Convert.ToInt32(item["Id"]);
                        activity.Description = item["Description"].ToString();
                        activity.Duration = Convert.ToInt32(item["Duration"]);
                        activity.Predecessors = GetPredecessors(item["Predecessors"].ToString());

                        activities.Add(activity);
                    }
                }
            }

            return activities;
        }

        private static List<int> GetPredecessors(string predecessorsString)
        {
            List<int> predecessors = new List<int>();

            if(!string.IsNullOrEmpty(predecessorsString))
            {
                predecessors = predecessorsString.Split(",").Select(int.Parse).ToList();
            }

            return predecessors;
        }
    }
}
