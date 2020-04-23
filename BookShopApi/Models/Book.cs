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

        public string bookAuthor { get; set; }

        public string bookTitle { get; set; }

        public string bookDescript { get; set; }

        public string bookPrice { get; set; }

        public string bookImg { get; set; }
    }
}