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
        override
        public void Next(Tick tick)
        {
            return;
        }

        override
        public void Notify(Order order)
        {
            return;
        }
    }
}
