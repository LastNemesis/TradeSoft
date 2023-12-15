using System;
using System.Collections;
using TradeSoft.Services;

namespace TradeSoft.Models
{
    public class Strategy
    {
        protected static int id_counter = 0;
        private int _id;
        private Broker _broker;

        public int Id
        {
            get { return _id; }
        }

        public Broker Broker
        {
            set { _broker = value; }
        }

        public Strategy()
        {
            _id = id_counter;
            id_counter += 1;
        }
        public virtual void Next(Tick tick)
        {
            return;
        }
        private void Sell(float quantity, float price)
        {
            Order order = new Order(Id, price, quantity, OrderType.sell, DateTime.Now);
            _broker.Sell(order);
        }

        private void Buy(float quantity, float price)
        {
            Order order = new Order(Id, price, quantity, OrderType.buy, DateTime.Now);
            _broker.Buy(order);
        }

        public void Close(Tick lastTick)
        {
            float position = _broker.GetPositionValue(this.Id);
            if (position > 0) //long position
            {
                Sell(position, lastTick.price);
            }
            else if (position < 0) //short position
            {
                Buy(position, lastTick.price);
            }
        }

        public virtual void Notify(Order order)
        //called after Order execution, to notify strategy of what was executed in the order
        {
            return;
        }
    }
}


