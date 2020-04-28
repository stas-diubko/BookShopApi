using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookShopApi.Models
{
    [BsonIgnoreExtraElements]
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public string author { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public string price { get; set; }

        public string bookImage { get; set; }
    }
}