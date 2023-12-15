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
    internal class DataService
    {
        public void FetchData()
        {
            using (var reader = new StreamReader("tradesoft-ticks-sample.csv"))
            {
                List<Tick> tickList = new List<Tick>();
                string line = reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    var values = line.Split(',');
                    DateTime date = Convert.ToDateTime(values[0]);
                    string type = values[1];
                    Int32 quantity = Convert.ToInt32(values[2]);
                    float price = Convert.ToSingle(values[3], System.Globalization.CultureInfo.InvariantCulture);
                    tickList.Add(new Tick(date, type, quantity, price));
                }
                Console.WriteLine(tickList.Count);
            }
        }
    }
}
