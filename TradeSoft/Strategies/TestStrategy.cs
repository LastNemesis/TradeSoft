using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSoft.Models;

namespace TradeSoft.Strategies
{
    internal class TestStrategy: Strategy
    {
        private Tick? lastTick;
        private float shares = 0;

        override
        public void Next(Tick tick)
        {
            if(lastTick != null) { 
                if(tick.price > lastTick.price && shares >= 1)
                {
                    Console.WriteLine("sell 1");
                    this.Sell(-1, tick.price);
                    shares--;
                } else
                {
                    Console.WriteLine("buy 1");
                    this.Buy(1, tick.price);
                    shares++;
                }
            }
            this.lastTick = tick;
            return;
        }

        override
        public void Notify(Order order)
        {
            //Console.WriteLine(order.ToString());
            return;
        }
    }
}
