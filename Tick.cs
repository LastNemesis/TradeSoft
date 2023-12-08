using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSoft
{
    public class Tick
    {
        public DateTime time;
        public string type;
        public int quantity;
        public float price;

        public Tick(DateTime time, string type, int quantity, float price)
        {
            this.time = time;
            this.type = type;
            this.quantity = quantity;
            this.price = price;
        }
    }
}
