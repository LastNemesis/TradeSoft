using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeSoft.Models
{
    //class representing the Position held by a Strategy in the maker
    public class Position
    {
        //Strategy id used to identified the corresponding strategy
        public int Strat_ID;

        //Value of the position
        private float _position;

        public float PositionValue
        {
            get { return _position; }
        }

        public Position(int Strat_ID)
        {
            _position = 0;
            this.Strat_ID = Strat_ID;
        }

        //method used to update the position based on an applied order, called by the Broker
        public void UpdatePosition(Order order)
        {
            _position += order.ExecutionBits[^1].Quantity;
        }
    }
}
