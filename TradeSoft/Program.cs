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
            string filePath = Path.Combine("..", "..", "..", "..", "TradeSoft", "Resources", "tradesoft-ticks-sample.csv");

            // Creating the dataService Object
            DataService dataService = new DataService();

            // Getting the Raw Data
            List<Tick> tickList = dataService.FetchData(filePath);

            // Displaying the number of lines
            Console.WriteLine(tickList.Count);

            // Timeframe wanted
            TimeSpan timeSpan = new TimeSpan(0, 30, 0);

            // Resampling the Raw Data
            List<Tick> resampledTickList = dataService.ResampleData(tickList, timeSpan);

            // Displaying the number of lines
            Console.WriteLine(resampledTickList.Count);
        }
    }
}