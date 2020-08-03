using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Models;
using TaskManager.Mongo;
using MongoDB.Driver;
using MongoDB.Bson.IO;

namespace TaskManager.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(ITaskManagerDatabaseSettings settings)
        {
            var client = MongoConnection.getInstance();
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(settings.UsersCollectionName);
        }

        public List<User> Get() => _users.Find(book => true).ToList();

        public User Get(string id) => _users.Find<User>(user => user.Id == id).FirstOrDefault();

        public User Create(User user){
            _users.InsertOne(user);
            return user;
        }

        public User Update(string id, User userIn)
        {
            _users.ReplaceOne(user => user.Id == id, userIn);
            return userIn;
        }

        public int Remove(string id)
        {
            DeleteResult dr = _users.DeleteOne(user => user.Id == id);
            return (Int32)dr.DeletedCount;
        }

        public int Remove(User userIn)
        {
            DeleteResult dr = _users.DeleteOne(user => user.Id == userIn.Id);
            return (Int32)dr.DeletedCount;
        }

        public static User VerifyUserWithToken(string _id, string token)
        {
            try
            {
                User user = _users.FindSync<User>(user => user.Id == _id && user.Tokens.Contains<string>(token)).Single();
                return user;
            }catch(Exception ex)
            {
                Console.WriteLine($"{ex.GetType()} :: {ex.Message}");
                return null;
            }
        }
    }
}
