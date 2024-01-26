using System;
using TradeSoft.Models;

namespace TradeSoft.Services
{
    internal class Engine
    {
        public void Run(DataService dataService)
        {
            Broker broker = new Broker();
            StrategyHandler strategyHandler = new StrategyHandler(broker);

            foreach(Tick tick in dataService.ticks())
            {
                Console.WriteLine(tick.ToString());
                broker.simulateTick(tick);
                strategyHandler.SendTick(tick); //to be made async ?

                //wait until next tick ? using Clock ?

                List<Order> tickOrders = broker.GetTickOrders();
                strategyHandler.NotifyStrategies(tickOrders);
                Console.WriteLine("\n");
            }

            //define how we use the ticks to have access to the last tick
            //maybe Strategies directly get market price from Broker in Close method ?
            strategyHandler.CloseStrategies(dataService.listTicks[^1]);

            List<Order> orders = broker.GetAllOrders();
            foreach(Order order in orders)
            {
                Console.WriteLine(order.ToString());
            }
            Console.WriteLine(orders.Count);
            //send orders to strategies

            //log results: orders/ticks/analysis results
            Analysis riskAnalysis = new Analysis();
            riskAnalysis.runMethods(orders);

            Console.WriteLine(riskAnalysis.ToString());
        }
    }
}
