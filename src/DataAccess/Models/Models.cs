using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace idb.Backend.DataAccess.Models
{
    public class User : MongoEntity
    {
        public string email { get; set; }
        public string password { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public bool is_admin { get; set; }
    }

    public class Item : MongoEntity
    {
        public string name { get; set; }
        public List<Tag> tags { get; set; }
        public string content { get; set; }
        public string ownerId { get; set; }
    }

    public class Tag : MongoEntity
    {
        public string name { get; set; }
    }

    public abstract class MongoEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string guid { get; set; }
        public int ID { get; set; }
        public DateTime timestamp { get; set; }
        public DateTime created_at { get; set; }
    }
}
