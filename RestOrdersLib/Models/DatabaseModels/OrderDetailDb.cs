using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RestOrdersLib.Models.DatabaseModels
{

    public class OrderDetailDb
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public int OrderId { get; set; }

        public DateTime CreatedTime { get; set; }

        public string EventType { get;set;}

        public string OrderDetails { get; set;}
    }
}