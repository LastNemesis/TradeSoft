using System.IO;
using System;

namespace Main
{
    class Program
    {
        static void Main(string[] args)
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
                    tickList.Add(new Tick(date,type,quantity,price));
                }
                Console.WriteLine(tickList.Count);
            }
        }
    }
}

class Tick
{
    public DateTime time;
    public string type;
    public int quantity;
    public float price;

    public Tick(DateTime time, string type, int quantity, float price)
    {
        this.time = time;
        this.type = type;
        this.quantity = quantity;
        this.price = price;
    }
}

