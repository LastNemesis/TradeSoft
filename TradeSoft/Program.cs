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

            Engine engine = new Engine();
            engine.Run(dataService);
        }
    }
}