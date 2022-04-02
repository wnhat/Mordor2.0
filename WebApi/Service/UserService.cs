using CoreClass.Model;
using WebApi.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace WebApi.Service
{ 
    public interface IUserService
    {
        Task<User> Authenticate(string account, string password);
        Task<User> CreateUser(User user, string password);
        Task<List<User>> GetAll();
        Task<User> GetUserById(string id);
        Task<User> GetUserByAccount(string account);
        Task UpdateUserInfo(User userParam, string password = null);
        Task DeleteUser(string account);
    }

    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IUserDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(settings.UsersCollectionName.User);
        }

        public Task<User> Authenticate(string account, string password)
        {
            return Task.Run(() =>
            {
                User user = null;
                if (!string.IsNullOrEmpty(account) && !string.IsNullOrEmpty(password))
                {
                    var filter = Builders<User>.Filter.Eq("Account", account);
                    user = _users.Find(filter).FirstOrDefault();
                }
                if (user != null && user.VerifyPasswordHash(password))
                {
                    return user;
                }

                return null;
            });
        }

        public Task<User> CreateUser(User user, string password)
        {
            return Task.Run(() =>
            {
                var filter = Builders<User>.Filter.Eq("Account", user.Account);
                if (string.IsNullOrWhiteSpace(password))
                {
                    throw new ApplicationException("Password is required");
                }
                if (_users.Find(filter).FirstOrDefault() != null)
                {
                    throw new ApplicationException("Username \"" + user.Account + "\" is already taken");
                }

                string[] split = user.Organization.Split('_');
                user.CreatePasswordHash(password);
                user.JudgeUserWeight(split[0]);
                _users.InsertOne(user);
                return user;
            });
        }

        public Task DeleteUser(string account)
        {
            return Task.Run(() => { return _users.DeleteOne(user => user.Account == account); });
        }

        public Task<List<User>> GetAll()
        {
            return Task.Run(() => { return _users.Find(user => true).ToList(); });
        }

        public Task<User> GetUserById(string id)
        {
            var filter = Builders<User>.Filter.Eq("Id", id);
            return Task.Run(() => { return _users.Find(filter).FirstOrDefault(); });
        }

        public Task<User> GetUserByAccount(string account)
        {
            var filter = Builders<User>.Filter.Eq("Account", account);
            return Task.Run(() => { return _users.Find(filter).FirstOrDefault(); });
        }

        public Task UpdateUserInfo(User userParam, string password = null)
        {
            return Task.Run(() =>
            {
                var filter = Builders<User>.Filter.Eq("Id", userParam.Id);
                var user = _users.Find(filter).FirstOrDefault();

                if (user == null)
                {
                    throw new ApplicationException("User not found");
                }      
                if (user.VerifyPasswordHash(password))
                {
                    throw new ApplicationException("Can't modify the same password");
                }
                if (!string.IsNullOrWhiteSpace(password))
                {
                    user.CreatePasswordHash(password);
                    var update = Builders<User>.Update.Set("PasswordHash", user.PasswordHash).Set("passwordSalt", user.PasswordSalt);
                    _users.UpdateOne(filter, update);
                }
                
            });
        }

    }

}
