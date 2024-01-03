using System.IO;
using System;

using TradeSoft.Models;
using TradeSoft.Services;
using TradeSoft.Strategies;

namespace Main
{
    class Program
    {
        static void Main(string[] args)
        {
            // Getting the path of the CSV
            string filePath1 = Path.Combine("..", "..", "..", "..", "TradeSoft", "Resources", "tradesoft-ticks-sample.csv");

            // Creating the dataService Object
            DataService dataService = new DataService();

            // Getting the Raw Data
            List<Tick> tickList = dataService.FetchData(filePath1);

            // Displaying the number of lines
            Console.WriteLine(tickList.Count);

            // Timeframe wanted
            TimeSpan timeSpan = new TimeSpan(0, 1, 0);

            // Resampling the Raw Data
            List<Tick> resampledTickList = dataService.ResampleData(tickList, timeSpan);

            // Displaying the number of lines
            Console.WriteLine(resampledTickList.Count);

            // Getting the path of the CSV
            string filePath2 = Path.Combine("..", "..", "..", "..", "TradeSoft", "Resources", "tradesoft-ticks-resample.csv");

            // Writing the resampled data inside a new CSV
            dataService.WriteData(resampledTickList, filePath2);
        }
    }
}