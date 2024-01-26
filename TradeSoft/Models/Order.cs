using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSoft.Models
{
    /* Class ORDER
     * This class encapsule all information that are necessary to make an order (buy or sell)
     * 
     * Order instances are created by the Strategy class, when an instance of strategy makes a decision.
     * 
     * The order instance created is sent to the broker who is in charge of the execution.
     * 
     * The order may or may not be executed. 
     * When executed, its execution may have different values (price or quantity different from demand). 
     * This is why this class also has an ExecutedData attribute, which refers to what was actually executed in comparison
     * to what was requeste
     */
    public class Order
    {
        // Static field to keep track of the last assigned ID
        private static int id_counter = 0;

        // Unique identifier of the order
        private int _orderId;

        // Unique identifier of the strategy behind this order
        private int _stratId;

        // Order price
        private float _price;

        // Could be named size
        private float _quantity;

        // Buy or sell
        private OrderType _type;

        // Order time-stamp
        private DateTime _dt;

        // Execution data
        private ExecutionData _executionData;

        // Properties, getters, and setters
        public int OrderId
        {
            get { return _orderId; }
        }

        public int StratId
        {
            get { return _stratId; }
            set { _stratId = value; }
        }

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

        public DateTime DT
        {
            get { return _dt; }
            set { _dt = value; }
        }

        public ExecutionData EData
        {
            get { return _executionData; }
            set { _executionData = value; }
        }

        public Order(int stratId, float price, float quantity, OrderType type, DateTime dt)
        {
            // Assigning a unique order ID
            _orderId = ++id_counter;

            _stratId = stratId;
            _price = price;
            _quantity = quantity;
            _type = type;
            _dt = dt;
        }

        public override string ToString()
        {
            return $"Order ID: {_orderId}, Strategy ID: {_stratId}, Price: {_price}, Quantity: {_quantity}";
        }
    }


    public enum OrderType
    {
        buy,
        sell
    }
}
