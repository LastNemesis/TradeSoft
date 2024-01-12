using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSoft.Models
{
    public class Tick
    {
        public DateTime time;

        public string type;

        public int quantity;

        public float price;

        override
        public String ToString()
        {
            return String.Format("time: {0} quantity: {1} price {2}", time, quantity, price);
        }

        // Constructor
        public Tick(DateTime time, string type, int quantity, float price)
        {
            this.time = time;

            this.type = type;

            this.quantity = quantity;

            this.price = price;
        }
    }
}
