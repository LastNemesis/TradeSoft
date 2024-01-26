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
            string logMessage = $"Order: OrderId: {order.OrderId}, StrategyId: {order.StratId}, Price: {order.Price}, Quantity: {order.Quantity}, DateTime: {order.DT}";
            WriteToFile(logMessage);
        }

        public void LogExecutedOrder(Order order)
        {
            ExecutionData executionData = order.EData;
            string logMessage = $"Executed Order: OrderId: {order.OrderId}, Status: {order.Status}, Price: {executionData.Price}, Quantity: {executionData.Quantity}, DateTime: {executionData.DT}";
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