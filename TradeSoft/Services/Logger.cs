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

        public void LogExecutedOrder(ExecutionBit executionBit)
        {
            string logMessage = $"Executed Bit: StrategyId: {executionBit.Id}, Price: {executionBit.Price}, Quantity: {executionBit.Quantity}, DateTime: {executionBit.DT}";
            WriteToFile(logMessage);
        }

        public void LogAnalysis(Order order, String analysis)
        {
            string logMessage = $"Analysis: Strategy: {order.StratId}, OrderId: {order.OrderId} {analysis}";
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