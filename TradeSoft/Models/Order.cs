using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
        protected static int id_counter = 0;

        // Unique identifier of the order
        protected int _orderId;

        // Unique identifier of the strategy behind this order
        protected int _stratId;

        // Order price
        private float _price;

        // Could be named size
        protected float _quantity;

        private List<ExecutionBit> _executionBits = new List<ExecutionBit>();

        // Buy or sell
        protected OrderType _type;

        // Status of the Order Completion
        protected OrderStatus _status;

        // Order time-stamp
        protected DateTime _dt;

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

        public ExecutionData ExecutionData
        {
            get { return _executionData; }
            set { _executionData = value; }
        }

        public List<ExecutionBit> ExecutionBits
        {
            get { return _executionBits;  }
            set { _executionBits = value; }
        }

        public OrderStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public OrderType Type
        {
            get { return _type; }
        }

        public Order(int stratId, float quantity, DateTime dt)
        {
            // Assigning a unique order ID
            _orderId = ++id_counter;

            _stratId = stratId;
            _quantity = quantity;
            _dt = dt;
        }
    }

    public class MarketOrder : Order
    {
        public MarketOrder(int stratId, float quantity, DateTime dt) : base(stratId, quantity, dt)
        {
            _type = OrderType.Market;
        }

        public override string ToString()
        {
            return $"MarketOrder ID: {_orderId}, Strategy ID: {_stratId}, Quantity: {_quantity}";
        }
    }
    public enum OrderType
    {
        Market,
        Limit,
        Stop,
        StopLimit
    }

    public enum OrderStatus
    {
        submitted,
        accepted,
        partial,
        completed,
    }
}
