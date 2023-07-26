namespace OrdersEventHandler.Models
{
    public class OrderNotification
    {
        public int OrderId { get; set; }

        public string EventType { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}
