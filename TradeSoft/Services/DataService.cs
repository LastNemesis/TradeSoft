using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using TradeSoft.Models;


namespace TradeSoft.Services
{
    public class DataService
    {
        public List<Tick> FetchData(string filePath)
        {

            // Creating the list of ticks
            List<Tick> tickList = new List<Tick>();

            // Loading the CSV file containing the sample market data
            using (var reader = new StreamReader(filePath))
            {

                // Creating the variable containing a tick, and loading the first line
                string? line = reader.ReadLine();

                // Creating the loop that reads every line until the last empty line
                while ((line = reader.ReadLine()) != null)
                {
                    
                    // Separating the values 
                    var values = line.Split(',');
                    
                    // Getting the date of the tick
                    DateTime date = Convert.ToDateTime(values[0]);

                    // Getting the type of the tick
                    string type = values[1];

                    // Getting the 'quantity' value
                    Int32 quantity = Convert.ToInt32(values[2]);

                    // Getting the 'price' value
                    float price = Convert.ToSingle(values[3], System.Globalization.CultureInfo.InvariantCulture);

                    // Adding the three values into the Tick list
                    tickList.Add(new Tick(date, type, quantity, price));
                }
                
            }

            // Returning the Tick list
            return tickList;
        }
    }
}
