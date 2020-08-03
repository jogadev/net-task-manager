using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TaskManager.Mongo
{
    public class MongoConnection
    {
        static MongoClient mongoClient;
        readonly static string connectionString = "mongodb+srv://superman:DQOe7lmgOq6cgyHu@cluster0-4eopb.azure.mongodb.net/task-manager?retryWrites=true&w=majority";
        public static MongoClient getInstance()
        {
            if (mongoClient == null)
                mongoClient = new MongoClient(connectionString);
            return mongoClient;
        }
    }
}
