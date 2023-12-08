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
        private int Strat_ID { get; set; }
        
        // Order price
        private float Price { get; set; }
        
        // Could be named size
        private float Quantity { get; set; }
        
        // Buy or sell
        private OrderType Type { get; set; }
        
        // Order time-stamp
        private DateTime DT { get; set; }
        
        // Execution data
        private ExecutionData executionData { get; set; }
        

        public Order(int strat_ID, float price, float quantity, OrderType type, DateTime dt)
        {
            
            Strat_ID = strat_ID;
            
            Price = price;
            
            Quantity = quantity;
            
            Type = type;
            
            DT = dt;
        }
    }


    enum OrderType
    {
        buy,
        sell
    }
}
