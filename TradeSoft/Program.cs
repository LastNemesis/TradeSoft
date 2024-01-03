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

            // Displaying the number of lines
            Console.WriteLine(dataService.FetchData(filePath).Count);
        }
    }
}