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
            List<Tick> tickList = new();

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

        public void WriteData(List<Tick> tickList, string filePath)
        {
            // Checking if the list is not null or empty
            if (tickList == null || tickList.Count == 0)
            {
                return;
            }

            // Writing into the CSV file 
            using (var writer = new StreamWriter(filePath))
            {
                // Writing header line if needed
                writer.WriteLine("Time,Type,Quantity,Price");

                // Writing each tick as a line in the CSV file
                foreach (var tick in tickList)
                {
                    
                    // Creating the line to insert inside the CSV
                    string line = $"{tick.time.ToString("dd-MM-yyyy HH:mm:ss")},{tick.type},{tick.quantity},{tick.price.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
                    
                    // Writing the line inside the CSV
                    writer.WriteLine(line);
                }
            }
        }

        public List<Tick> ResampleData(List<Tick> initialTickList, TimeSpan timeSpan)
        {
            // Creating the list of ticks
            List<Tick> tickList = new();

            // Checking if the list is not null or empty
            if (initialTickList == null || initialTickList.Count == 0)
            {
                return tickList;
            }

            // Sort the initialTickList by time
            //initialTickList.Sort((a, b) => a.time.CompareTo(b.time));

            // Determine the start and end time for resampling
            DateTime startTime = initialTickList[0].time;
            DateTime endTime = initialTickList[initialTickList.Count - 1].time;

            // Loop through 3 time intervals defined by timeFrame for Testing purposes
            //for (DateTime currentTime = startTime; currentTime <= startTime + timeSpan + timeSpan; currentTime += timeSpan)

            // Loop through time intervals defined by timeFrame
            for (DateTime currentTime = startTime; currentTime <= endTime; currentTime += timeSpan)
                {
                // Creating the list of possible Ticks within the current time frame
                var ticksWithinInterval = initialTickList.Where(tick => tick.time >= currentTime && tick.time < currentTime + timeSpan).ToList();

                // Creating the new Tick that contains the values of the Ticks contained within the interval
                if (ticksWithinInterval.Any())
                {
                    // Creating the total quantity
                    int totalQuantity = ticksWithinInterval.Sum(t => t.quantity);

                    // Creating the price by creating the average of the price within the interval
                    float price = ticksWithinInterval.Sum(t => t.price) / ticksWithinInterval.Count;

                    // Creating the price by taking the price of the first Tick
                    //float price = ticksWithinInterval.First().price;

                    // Create a new tick with aggregated data for the interval
                    Tick resampledTick = new Tick(currentTime, "ResampledType", totalQuantity, price);

                    // Adding the new Tick inside the list
                    tickList.Add(resampledTick);
                }
                // Creating a placeholder tick, if there are no tick contained within the interval
                else
                {
                    // Adding the placeholder Tick inside the list
                    tickList.Add(new Tick(currentTime, "Empty", 0, 0));
                }
            }

            // Returning the resampled Tick list
            return tickList;
        }
    }
}