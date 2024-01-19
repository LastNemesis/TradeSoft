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
        private List<Tick>? resampledTickList;

            // Time Variables
        private DateTime startTime;
        private TimeSpan timeSpan;

            // Market opening and closing time
        private TimeSpan marketStartTime = new TimeSpan(9, 0, 0);   // 9:00 AM
        private TimeSpan marketEndTime = new TimeSpan(18, 0, 0);    // 6:00 PM

        // Constructor
        public DataService(TimeSpan timeSpan)
        {
            this.timeSpan = timeSpan;
            this.resampledTickList = new List<Tick>();
            this.startTime = DateTime.MinValue;
        }

        // Processing Tick Function
        public void ProcessTick(Tick tick)
        {
            // Update the start time if it's the first tick
            if (startTime == DateTime.MinValue)
            {
                startTime = tick.time;
            }

            // Check if the market is open or closed based on the hour
            if (marketStartTime <= tick.time.TimeOfDay && tick.time.TimeOfDay < marketEndTime)
            {
                // Check if we need to resample based on the time span
                while (tick.time >= startTime + timeSpan)
                {
                    ResampleData();
                }

                // Add the tick to the resampled list
                resampledTickList.Add(tick);
            }
        }

        // Function that resample the Data
        private void ResampleData()
        {
            // Get ticks within the current time frame
            var ticksWithinInterval = resampledTickList
                .Where(t => t.time >= startTime && t.time < startTime + timeSpan)
                .ToList();

            // Create the resampled tick if there are ticks within the interval
            if (ticksWithinInterval.Any())
            {
                int totalQuantity = ticksWithinInterval.Sum(t => t.quantity);
                float price = ticksWithinInterval.Sum(t => t.price) / ticksWithinInterval.Count;

                // Create a new tick with aggregated data for the interval
                Tick resampledTick = new Tick(startTime, "ResampledType", totalQuantity, price);

                // Clear ticks within the interval as they are now aggregated
                resampledTickList.RemoveAll(t => t.time >= startTime && t.time < startTime + timeSpan);

                // Add the resampled tick to the list
                resampledTickList.Add(resampledTick);
            }
            else
            {
                // Add a placeholder tick if there are no ticks within the interval
                resampledTickList.Add(new Tick(startTime, "Empty", 0, 0));
            }

            // Update the start time for the next interval
            startTime += timeSpan;
        }

        public List<Tick> GetResampledData()
        {
            // Ensure that the last interval is processed
            ResampleData();

            return resampledTickList;
        }

        // Write the Data inside a CSV
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

        // TEMP Function, that shows how the CSV is dynamically processed
        public List<Tick> DynamicallyLoadAndResample(string filePath)
        {
            // Open the CSV file and process each tick
            using (var reader = new StreamReader(filePath))
            {
                // Skip header line
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    // Getting the line
                    var line = reader.ReadLine();

                    // Getting the values of the line
                    var values = line.Split(',');

                    // Setting the values with its corresponding type
                    DateTime date = Convert.ToDateTime(values[0]);
                    string type = values[1];
                    int quantity = Convert.ToInt32(values[2]);
                    float price = Convert.ToSingle(values[3], System.Globalization.CultureInfo.InvariantCulture);

                    // Setting the Tick element with the corresponding values
                    Tick tick = new Tick(date, type, quantity, price);

                    // Process each tick as it arrives
                    ProcessTick(tick);
                }
            }

            // Get the final resampled data and returning it
            return GetResampledData();
        }

    }
}