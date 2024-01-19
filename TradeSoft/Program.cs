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
            /// Getting the path of the CSV
            string filePath = Path.Combine("..", "..", "..", "..", "TradeSoft", "Resources", "tradesoft-ticks-sample.csv");

            // Timeframe wanted (HH, MM, SS)
            TimeSpan timeSpan = new TimeSpan(0, 0, 1);

            // Creating the dataService Object with a time span of 1 second
            DataService dataService = new DataService(timeSpan);

            // Getting the Raw Data
            List<Tick> tickList = dataService.FetchData(filePath1);

            // Displaying the number of lines
            Console.WriteLine(tickList.Count);

            // Resampling the Raw Data
            List<Tick> resampledTickList = dataService.ResampleData(tickList, timeSpan);

            // Displaying the number of lines
            Console.WriteLine(resampledTickList.Count);

            // Getting the path of the CSV
            string filePath2 = Path.Combine("..", "..", "..", "..", "TradeSoft", "Resources", "tradesoft-ticks-resample.csv");

            // Writing the resampled data inside a new CSV
            dataService.WriteData(resampledTickList, filePath2);

            // Creating the engine Object
            //Engine engine = new Engine();

            // Running the engine with the resampled data
            //engine.Run(resampledTickList);
        }
    }
}