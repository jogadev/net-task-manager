using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskManager.Models
{
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("password")]
        public string Password { get; set; }
        [BsonElement("tokens")]
        public List<string> Tokens { get; set; }
        [BsonElement("avatar")]
        public string Avatar { get; set; }
        [BsonElement("age")]
        public int Age { get; set; }
        [BsonElement("email")]
        public string Email { get; set; }

    }
}