namespace RestOrdersLib.Models.DatabaseModels
{
    public class OrderDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string OrderDetailCollectionName { get; set; } = null!;
    }
}
