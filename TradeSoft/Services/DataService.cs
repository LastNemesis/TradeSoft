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
        // Defining key variables
            // Resample Tick List
        private List<Tick>? processedTickList;

            // Time Variables
        private DateTime startTime;
        private TimeSpan timeSpan;

        // Market opening and closing time
        private TimeSpan marketStartTime = new TimeSpan(9, 0, 0);   // 9:00 AM
        private TimeSpan marketEndTime = new TimeSpan(18, 0, 0);    // 6:00 PM

        /// Getting the path of the input CSV
        string filePath_in = Path.Combine("..", "..", "..", "..", "TradeSoft", "Resources", "tradesoft-ticks-sample.csv");

        // Getting the path of the output CSV
        string filePath_out = Path.Combine("..", "..", "..", "..", "TradeSoft", "Resources", "tradesoft-ticks-resample.csv");

        // Constructor
        public DataService()
        {
            // The timespan between each resample tick is defined here
            this.timeSpan = new TimeSpan(0, 1, 0); ;

            // The resampled tick list
            this.processedTickList = new List<Tick>();

            // The start time
            this.startTime = DateTime.MinValue;
        }

        // Dynamic return tick function
        public IEnumerable<Tick> ticks()
        {
            // Return list
            List<Tick> ticks = new List<Tick>();
            
            // Returning the processd tick when ask for it
            foreach (var tick in ProcessTick())
            {
                if (tick != null)
                {
                    yield return tick;
                }
            }
        }

        // Static return all ticks function
        public List<Tick> GetAllProccessedTicks()
        {
            // Return list
            List<Tick>? tickList = new List<Tick>();

            // Getting all the Proccessed Ticks
            foreach (var tick in ProcessTick())
            {
                if (tick != null)
                {
                    tickList.Add(tick);
                }
            }

            // Returning the list
            return tickList;
        }

        // FetchData function
        public IEnumerable<Tick> FetchData()
        {
            // Open the CSV file and process each tick
            using (var reader = new StreamReader(filePath_in))
            {
                // Skip header line
                reader.ReadLine();

                // Reading each of the lines
                while (!reader.EndOfStream)
                {
                    // Getting the current line
                    var line = reader.ReadLine();

                    // Getting the values of the line
                    var values = line.Split(',');

                    // Setting the values with its corresponding type
                        // Date
                    DateTime date = Convert.ToDateTime(values[0]);

                        // Type
                    string type = values[1];

                        // Quantity
                    int quantity = Convert.ToInt32(values[2]);

                        // Price
                    float price = Convert.ToSingle(values[3], System.Globalization.CultureInfo.InvariantCulture);

                    // Creating a Tick with the fetched values
                    Tick tick = new Tick(date, type, quantity, price);

                    yield return tick;
                }
            }
        }

        // ProcessTick function
        public IEnumerable<Tick> ProcessTick()
        {
            foreach (var tick in FetchData())
            {
                // Update the start time at the first tick
                if (startTime == DateTime.MinValue)
                {
                    startTime = tick.time;
                }

                // Check if the market is open or closed based on the hour
                if (marketStartTime <= tick.time.TimeOfDay && tick.time.TimeOfDay <= marketEndTime)
                {
                    // Adding the tick to the toResample ticklist
                    processedTickList.Add(tick);

                    // Check if we need to resample based on the time span
                    while (tick.time >= startTime + timeSpan)
                    {
                        ResampleTicks();
                    }

                    // Checking if the returned tick has been processed
                    if (processedTickList.Last().type == "ProccessedType")
                    {
                        // Returning processed tick
                        yield return processedTickList.Last();
                    
                    } else {
                        // Returning null reference
                        yield return null;

                    }
                }
            }

            // returning last found tick
            yield return processedTickList.Last();

        }

        // Function that resample the Data
        private Tick ResampleTicks()
        {

            // Get ticks within the current time frame
            var ticksWithinInterval = processedTickList
                .Where(t => startTime <= t.time && t.time < startTime + timeSpan)
                .ToList();

            // Creation of the variable resampledTick
            Tick resampledTick;

            // Create the resampled tick if there are ticks within the interval
            if (ticksWithinInterval.Any())
            {
                int totalQuantity = ticksWithinInterval.Sum(t => t.quantity);
                float price = ticksWithinInterval.Sum(t => t.price) / ticksWithinInterval.Count;

                // Create a new tick with aggregated data for the interval
                resampledTick = new Tick(startTime, "ProccessedType", totalQuantity, price);

                // Clear ticks within the interval as they are now aggregated
                processedTickList.RemoveAll(t => startTime <= t.time && t.time < startTime + timeSpan);

                // Add the resampled tick to the list
                processedTickList.Add(resampledTick);
            }
            else
            {
                // Create a new tick with empty data
                resampledTick = new Tick(startTime, "Empty", 0, 0);

                // Add a placeholder tick if there are no ticks within the interval
                processedTickList.Add(resampledTick);
            }

            // Update the start time for the next interval
            startTime += timeSpan;

            // returning the aggretated Tick
            return resampledTick;
        }

        // Write the Data inside a CSV
        public void WriteData()
        {
            // Calling the GetAllProccessedTicks function
            List<Tick> tickList = GetAllProccessedTicks();

            // Checking if the list is not null or empty
            if (tickList == null || tickList.Count == 0)
            {
                return;
            }

            // Writing into the CSV file 
            using (var writer = new StreamWriter(filePath_out))
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

    }
}