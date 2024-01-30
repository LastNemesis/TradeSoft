﻿using System;
using TradeSoft.Models;

namespace TradeSoft.Services
{
    internal class Engine
    {
        public void Run(DataService dataService)
        {
            Logger logger = new Logger(Path.Combine("..", "..", "..", "..", "TradeSoft", "Logs", "log.txt"));
            Broker broker = new Broker(logger);
            StrategyHandler strategyHandler = new StrategyHandler(broker);
            AnalysisHandler analysisHandler = new AnalysisHandler(strategyHandler.GetStrategiesId(), logger);

            broker.OrderExecuted += strategyHandler.NotifyStrategies;
            //broker.OrderExecuted += analysisHandler.GetMethod

            foreach (Tick tick in dataService.ticks())
            {
                Console.WriteLine(tick.ToString());
                logger.LogTick(tick);
                broker.simulateTick(tick);
                strategyHandler.SendTick(tick);


                List<Order> tickOrders = broker.GetTickOrders();

                analysisHandler.analyseExecutionBits(tickOrders);
            }

            Console.WriteLine("end ?");
            //define how we use the ticks to have access to the last tick
            //maybe Strategies directly get market price from Broker in Close method ?
            //strategyHandler.CloseStrategies(dataService.listTicks[^1]);
        }
    }
}
