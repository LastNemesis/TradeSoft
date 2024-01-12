using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSoft.Models
{
    public class ExecutionData
    {
        private float _price;
        private float _quantity;
        private OrderType _type;
        private DateTime _dt;

        //properties : getters and setters for each field
        public float Price
        {
            get { return _price; }
            set { _price = value; }
        }

        public float Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }

        public OrderType Type
        {
            get { return _type; }
            set { _type = value; }
        }
        public DateTime DT
        {
            get { return _dt; }
            set { _dt = DateTime.Now; }
        }

        public ExecutionData(float price, float quantity, DateTime dt)
        {
            _price = price;

            _quantity = quantity;

            _dt = dt;
        }
    }
}
