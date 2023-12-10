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
        // Unique identifier of the strategy behind his order
        private int _strat_ID;

        // Order price
        private float _price;

        // Could be named size
        private float _quantity;

        // Buy or sell
        private OrderType _type { get; set; }

        // Order time-stamp
        private DateTime _dt;

        // Execution data
        private ExecutionData _executionData;


        //All preperties, getter and setter of each field
        public int Strat_ID
        {
            get { return _strat_ID; }
            set { _strat_ID = value; }
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
            set { _dt = DateTime.Now; }
        }

        public ExecutionData executionData
        {
            get { return _executionData; }
            set { _executionData = value; }
        }



        private Order(int strat_ID, float price, float quantity, OrderType type, DateTime dt)
        {
            
            _strat_ID = strat_ID;
            
            _price = price;
            
            _quantity = quantity;
            
            _type = type;
            
            _dt = dt;
        }
    }


    enum OrderType
    {
        buy,
        sell
    }
}
