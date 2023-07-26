using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrdersEventHandler.Models.DatabaseModels
{
    [Table("Products")]
    public class ProductDb
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        [ForeignKey("OrderId")]
        public virtual OrderDb Order { get; set; }
    }
}
