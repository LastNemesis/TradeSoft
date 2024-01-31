using System;
using System.IO;
using TradeSoft.Models;

namespace TradeSoft.Services
{
    public class Logger
    {
        private readonly string filePath;
        private List<string> logs = new List<string>();

        public Logger(string filePath)
        {
            this.filePath = filePath;
        }

        public void LogTick(Tick tick)
        {
            string logMessage = $"Tick: {tick.time}, Quantity: {tick.quantity}, Price: {tick.price}";
            logs.Add(logMessage);
        }

        public void LogOrder(Order order)
        {
            string logMessage = $"Order: {order}";
            logs.Add(logMessage);
        }

        public void LogExecutedBit(ExecutionBit executionBit)
        {
            string logMessage = $"Executed Bit: StrategyId: {executionBit.Id}, Price: {executionBit.Price}, Quantity: {executionBit.Quantity}, DateTime: {executionBit.DT}";
            logs.Add(logMessage);
        }

        public void LogAnalysis(ExecutionBit executionBit, String analysis)
        {
            string logMessage = $"Analysis: Strategy: {executionBit.Id}, {analysis}, Time: {executionBit.DT}";
            logs.Add(logMessage);
        }

        public void Log()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    foreach (string line in logs)
                    {
                        writer.WriteLine(line);
                    }
                }
                logs.Clear();
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }
    }
}