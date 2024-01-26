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
        private List<Order> _ordersLastTick = new List<Order>();

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
        public void simulateTick(Tick tick)
        {
            _ordersLastTick = new List<Order>();
            _marketPrice = tick.price;
        }

        //called in the case of a Sell order
        public void Sell(Order order) {
            logger.LogOrder(order);
            Console.WriteLine(order.ToString());
            ExecutionData EData = new ExecutionData(order.Price, order.Quantity, DateTime.Now);
            order.EData = EData;

            ApplyOrder(order);
        }

        //called in the case of a Buy order
        public void Buy(Order order) {
            logger.LogOrder(order);
            Console.WriteLine(order.ToString());
            ExecutionData EData = new ExecutionData(order.Price, order.Quantity, DateTime.Now);
            order.EData = EData;

            ApplyOrder(order);
        }

        //called by Sell and Buy function to update the position and store the applied order
        public void ApplyOrder(Order order)
        {
            logger.LogExecutedOrder(order);
            Position position = GetPosition(order.StratId);
            position.UpdatePosition(order);

            _ordersLastTick.Add(order);
            _orders.Add(order);
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

        //used to get all order for the current Tick of execution
        public List<Order> GetTickOrders()
        {
            return _ordersLastTick;
        }

        //used to get all orders
        public List<Order> GetAllOrders()
        {
            return _orders;
        }
    }
}
