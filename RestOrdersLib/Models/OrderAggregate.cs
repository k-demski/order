
namespace RestOrdersLib.Models
{
    public class OrderAggregate
    {
        public int OrderId { get; set; }

        public List<Product> Products { get; set; }

        public decimal TotalPrice { get; set; }
    }

    public class Product
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}
