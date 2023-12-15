using System;
using System.Collections;
using static System.Net.Mime.MediaTypeNames;
using static TradeSoft.Models.OrderType;
using static TradeSoft.Models.Order;
using static TradeSoft.Models.Tick;
using static TradeSoft.Broker;
using TradeSoft.Models;

namespace TradeSoft.Services
public class Strategy
{
    public class Strategy
    {
        protected static int id_counter = 0;
        private int Id { get; set; }
        private Broker Broker { get; set; }
        public Strategy()
        {
            Id = Strategy.id_counter;
            Strategy.id_counter += 1;
        }
        public virtual void Next(Tick tick)
        {
            return;
        }
        private void Sell(int quantity, float price)
        {
            Order order = new Order(this.Id, price, quantity, OrderType.sell, DateTime.Now);
            this.Broker.sell(order);
        }

        private void Buy(int quantity, float price)
        {
            Order order = new Order(this.Id, price, quantity, OrderType.buy, DateTime.Now);
            this.Broker.buy(order);
        }

        public void Close(Tick lastTick)
        {
            int position = this.Broker.GetPosition(this.Id);
            if (position > 0) //long position
            {
                this.Sell(position, lastTick.price);
            }
            else if (position < 0) //short position
            {
                this.Buy(position, lastTick.price);
            }
        }

        public virtual void Notify(Order order)
        //called after Order execution, to notify strategy of what was executed in the order
        {
            return;
        }
    }
}


