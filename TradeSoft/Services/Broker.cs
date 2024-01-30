using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TradeSoft.Models;

namespace TradeSoft.Services
{
    public class Broker
    {
        //store all the orders sent to the broker
        private List<Order> _orders = new List<Order>();

        //store all the orders in the current Tick of exectution
        private List<Order> _pendingOrders = new List<Order>();

        //store all Positions of strategies that sent an order
        private List<Position> _positions = new List<Position>();

        //store the marketPrice for the current Tick of execution
        private float _marketPrice = 0f;
        private Logger logger;

        public Broker(Logger logger)
        {
            this.logger = logger;
        }

        //called each tick to simulate the new Tick of execution to update market values
        public void SimulateTick(Tick tick)
        {
            _marketPrice = tick.price;
            Console.WriteLine(_pendingOrders.Count);

            foreach (Order order in _pendingOrders) {
                Console.WriteLine(order);
                if(order.Type == OrderType.Market)
                {
                    ExecutionBit executionBit = new ExecutionBit(order.StratId, _marketPrice, order.Quantity, DateTime.Now);
                    ApplyOrder(executionBit);
                    OrderExecuted?.Invoke(order.StratId, executionBit);
                }
            }

            _pendingOrders.Clear();
        }

        public void MarketOrder(int stratId, float quantity)
        {
            Order order = new MarketOrder(stratId, quantity, DateTime.Now);
            logger.LogOrder(order);
            _pendingOrders.Add(order);
        }

        //update the position
        public void ApplyOrder(ExecutionBit executionBit)
        {
            logger.LogExecutedBit(executionBit);
            Position position = GetPosition(executionBit.Id);
            position.UpdatePosition(executionBit);
        }

        //used to get the Position of a given Strategy
        public Position GetPosition(int Strat_ID)
        {
            foreach (Position position in _positions)
            {
                if (position.Strat_ID == Strat_ID)
                {
                    return position;
                }
            }

            return new Position(Strat_ID);
        }

        //used to get the position value of a given strategy
        public float GetPositionValue(int Strat_ID) 
        {
            foreach(Position position in _positions)
            {
                if(position.Strat_ID == Strat_ID)
                {
                    return position.PositionValue;
                }
            }

            return 0f; //if no position found for the strategy, it mean no position was ever taken
        }

        public event EventHandler<ExecutionBit> OrderExecuted;
    }
}
