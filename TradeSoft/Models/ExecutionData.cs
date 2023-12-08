using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSoft.Models
{
    public class ExecutionData
    {
        // Order price
        private float price { get; set; }
        
        // Could be named "Size"
        private float quantity { get; set; }
        
        // Order time
        private DateTime dt { get; set; }    //order time
    }
}
