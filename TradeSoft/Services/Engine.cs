using System;
using TradeSoft.Models;

namespace TradeSoft.Services
{
    internal class Engine
    {
        public void Run()
        {
            //temporary until we have a working Data Service
            Tick[] ticks = new Tick[10];

            Broker broker = new Broker();
            StrategyHandler strategyHandler = new StrategyHandler();

            foreach(Tick t in ticks)
            {
                broker.simulateTick(t);
                strategyHandler.SendTick(t); //to be made async ?

                //wait until next tick ? using Clock ?

                Order[] tickOrders = broker.GetTickOrders();
                strategyHandler.NotifyStrategies(tickOrders);
            }

            //define how we use the ticks to have access to the last tick
            //maybe Strategies directly get market price from Broker in Close method ?
            strategyHandler.CloseStrategies(ticks[^1]);

            Order[] orders = broker.GetAllOrders();

            //send orders to strategies

            //log results: orders/ticks/analysis results
        }
    }
}
