using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookShopApi.Models
{
    [BsonIgnoreExtraElements]
    public class Roles
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        public BsonArray users { get; set; }

        public BsonArray admins { get; set; }

        //public int __v { get; }
    }
}
