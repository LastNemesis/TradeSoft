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
        //store all the orders in the current Tick of exectution
        private List<Order> _pendingOrders = new List<Order>();

        //store all Positions of strategies that sent an order
        private List<Position> _positions = new List<Position>();

        //store the marketPrice for the current Tick of execution
        private float _marketPrice = 0f;
        private Tick _currentTick;
        private Logger logger;

        public Broker(Logger logger)
        {
            this.logger = logger;
        }

        //called each tick to simulate the new Tick of execution to update market values
        public void SimulateTick(Tick tick)
        {
            _currentTick = tick;
            _marketPrice = tick.price;
            Console.WriteLine(_pendingOrders.Count);
            List<Order> still_pending_orders = new List<Order>();

            foreach (Order order in _pendingOrders) {
                Console.WriteLine(order);
                if(order.Type == OrderType.Market)
                {
                    ExecutionBit executionBit = new ExecutionBit(order.StratId, _marketPrice, order.Quantity, _currentTick.time);
                    ApplyOrder(executionBit);
                    OrderExecuted?.Invoke(order.StratId, executionBit);
                } else if (order.Type == OrderType.Limit)
                {
                    if(order.Quantity < 0 && _marketPrice >= order.Price) {
                        ExecutionBit executionBit = new ExecutionBit(order.StratId, _marketPrice, order.Quantity, _currentTick.time);
                        ApplyOrder(executionBit);
                        OrderExecuted?.Invoke(order.StratId, executionBit);
                    } else if (order.Quantity > 0 && _marketPrice <= order.Price) { 
                        ExecutionBit executionBit = new ExecutionBit(order.StratId, _marketPrice, order.Quantity, _currentTick.time);
                        ApplyOrder(executionBit);
                        OrderExecuted?.Invoke(order.StratId, executionBit);
                    } else
                    {
                        still_pending_orders.Add(order);
                    }
                }
            }

            _pendingOrders.Clear();
            _pendingOrders.AddRange(still_pending_orders);
        }

        public void MarketOrder(int stratId, float quantity)
        {
            Order order = new MarketOrder(stratId, quantity, _currentTick.time);
            logger.LogOrder(order);
            _pendingOrders.Add(order);
        }

        public void LimitOrder(int stratId, float quantity, float limitPrice)
        {
            Order order = new LimitOrder(stratId, quantity, limitPrice, _currentTick.time);
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
