using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrdersEventHandler.Models.DatabaseModels
{
    [Table("Orders")]
    public class OrderDb
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        public ICollection<ProductDb> Products { get; set; }
    }
}