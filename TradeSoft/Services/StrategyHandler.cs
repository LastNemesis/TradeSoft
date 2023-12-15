using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TradeSoft.Models;
using TradeSoft.Strategies;
using System.Reflection;
using System.Reflection.Emit;

namespace TradeSoft.Services
{
    internal class StrategyHandler
    {
        private Strategy[] _strategies = new Strategy[0];

        public StrategyHandler()
        {
            LoadStrategies();
        }

        public void LoadStrategies()
        {
            //get all types in the namespace TradeSoft.Strategies
            Type[] strategies = Assembly.GetExecutingAssembly().GetTypes().Where(t => String.Equals(t.Namespace, "TradeSoft.Strategies", StringComparison.Ordinal))
              .ToArray();

            //Create an instance for each Strategy type found
            foreach (Type strategy in strategies) {
                Strategy instance = (Strategy)Activator.CreateInstance(strategy);
                _strategies.Append(instance);
            }
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
