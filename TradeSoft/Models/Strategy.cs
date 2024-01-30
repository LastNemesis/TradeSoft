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
            set 
            { 
                _broker = value ; 
            }
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
        protected void Sell(float quantity, float price = 0.0f, OrderType type = OrderType.Market)
        {
            if(type == OrderType.Market)
            {
                _broker.MarketOrder(Id, -quantity);
            }
        }

        protected void Buy(float quantity, float price = 0.0f, OrderType type = OrderType.Market)
        {
            if (type == OrderType.Market)
            {
                _broker.MarketOrder(Id, quantity);
            }
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

        public virtual void Notify(ExecutionBit executionBit)
        //called after Order execution, to notify strategy of what was executed in the order
        {
            return;
        }
    }
}


