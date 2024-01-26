namespace TradeSoft.Models
{
    public class ExecutionBit
    {
        // Order price
        private float _price;

        // Could be named "Size"
        private float _quantity;

        // Order time
        private DateTime _dt;

        //properties : getters and setters for each field
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

        public ExecutionBit(float price, float quantity, DateTime dt)
        {
            _price = price;

            _quantity = quantity;

            _dt = dt;
        }
    }
}