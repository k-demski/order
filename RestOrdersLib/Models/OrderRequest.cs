using RestOrdersLib.Validators;
using System.ComponentModel.DataAnnotations;

namespace RestOrdersLib.Models
{
    public class OrderRequest
    {
        [Required]
        public int OrderId { get; set; }

        [StringLength(6)]
        public string EventType { get; set; }

        [StringLength(10)]
        public string ProductName { get; set; }

        [PriceValidator("5000")]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
