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

        public Dictionary<string, Object> Create(User user) {
            var resDictionary = new Dictionary<string, Object>();
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            try
            {
                _users.InsertOne(user);
                resDictionary.Add("user", user);
            }
            catch (MongoWriteException mongoWriteException)
            {
                Console.WriteLine($"ERROR : {mongoWriteException.Message}");
                resDictionary.Add("error", mongoWriteException.Message) ;
            }
            return resDictionary;
        }

        public User Update(string id, User userIn)
        {
            // ENSURE THAT PASSWORD IS NOT DOUBLE-HASHED HERE
            try
            {
                _users.ReplaceOne(user => user.Id == id, userIn);
                
            }catch(MongoWriteException mongoWriteException)
            {
                return null;
            }
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

        public User VerifyUserWithToken(string _id, string token)
        {
            try
            {
                User user = _users.FindSync<User>(user => user.Id == _id && user.Tokens.Contains<string>(token)).FirstOrDefault();
                return user;
            } catch (Exception ex)
            {
                Console.WriteLine($"{ex.GetType()} :: {ex.Message}");
                return null;
            }
        }

    }
}
