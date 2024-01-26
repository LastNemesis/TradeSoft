using System;
using System.IO;
using TradeSoft.Models;

namespace TradeSoft.Services
{
    public class Logger
    {
        private readonly string filePath;

        public Logger(string filePath)
        {
            this.filePath = filePath;
        }

        public void LogTick(Tick tick)
        {
            string logMessage = $"Tick: {tick.time}, Quantity: {tick.quantity}, Price: {tick.price}";
            WriteToFile(logMessage);
        }

        public void LogOrder(Order order)
        {
            string logMessage = $"Order: Strategy ID: {order.Strat_ID}, Price: {order.Price}, Quantity: {order.Quantity}, DateTime: {order.DateTime}";
            WriteToFile(logMessage);
        }

        public void LogExecutedOrder(ExecutionData executionData)
        {
            string logMessage = $"Executed Order: Status: {executionData.Status}, Price: {executionData.Price}, Quantity: {executionData.Quantity}, DateTime: {executionData.DateTime}";
            WriteToFile(logMessage);
        }

        private void WriteToFile(string message)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine($"{DateTime.Now} - {message}");
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }
    }
}