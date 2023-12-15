using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TradeSoft.Models;

namespace TradeSoft.Services
{
    internal class Broker
    {
        public void Sell(Order myOrder) {}

        public void Buy(Order myOrder) {}

        public int GetPosition(int idOfStrat) 
        {
            return 0; //0 need to be change by the last position taken by the broker ordered by the strat
        }

    }
}
