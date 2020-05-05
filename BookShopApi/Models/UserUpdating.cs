
using MongoDB.Bson.Serialization.Attributes;

namespace BookShopApi.Models
{
    [BsonIgnoreExtraElements]
    public class UserUpdating
    {
        public string name { get; set; }
        
        public string email { get; set; }

        public string image { get; set; }
    }
}
