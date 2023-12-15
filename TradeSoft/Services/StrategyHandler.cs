using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSoft.Models;

namespace TradeSoft.Services
{
    internal class StrategyHandler
    {
        private Strategy[]? _strategies;

        public StrategyHandler()
        {
            LoadStrategies();
        }

        public void LoadStrategies()
        {

        }

        public void CloseStrategies(Tick lastTick)
        {
            if( _strategies == null )
            {
                return;
            }

            foreach (Strategy strategy in _strategies) {
                strategy.Close(lastTick);
            }
        }

        public void SendTick(Tick tick)
        {
            if (_strategies == null)
            {
                return;
            }

            foreach (Strategy strategy in _strategies)
            {
                strategy.Next(tick);
            }
        }

        public void SetBrocker(Broker broker)
        {
            if (_strategies == null)
            {
                return;
            }

            foreach(Strategy strategy in _strategies)
            {
                strategy.Broker = broker;
            }
        }

        public void NotifyStrategies(Order[] orders)
        {
            if (_strategies == null)
            {
                return;
            }

            foreach (Order order in orders)
            {
                foreach (Strategy strategy in _strategies)
                {
                    if(order.Strat_ID == strategy.Id)
                    {
                        strategy.Notify(order);
                    }
                }
            }
        }
    }
}
