using Microsoft.EntityFrameworkCore;

namespace OrdersEventHandler.Models.DatabaseModels
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions options)
        : base(options)
        { 
        }

        public virtual DbSet<OrderDb> Orders { get; set; }

        public virtual DbSet<ProductDb> Products { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<OrderDb>(entity =>
        //    {
        //        entity.HasKey(e => e.Id);
        //    });

        //    modelBuilder.Entity<ProductDb>(entity =>
        //    {
        //        entity.HasKey(e => e.Id);
        //        entity.HasOne(d => d.Order).WithMany(p => p.Products);
        //    });
        //}
    }
}
