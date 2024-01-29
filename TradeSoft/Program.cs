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
            
            // Creating the dataService Object
            DataService dataService = new DataService();

            // Writing the resampled data inside a new CSV
            dataService.WriteData();

            // Creating the engine Object
            //Engine engine = new Engine();

            // Running the engine with the resampled data
            //engine.Run(dataService);
        }
    }
}