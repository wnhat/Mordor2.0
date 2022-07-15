using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CoreClass.Model
{
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string Username { get; set; }
        public string Account { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Organization { get; set; } = null!;
        public string UserWeight { get; set; } = "5"!;

        public void CreatePasswordHash(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be empty or whitespace.", nameof(password));
            }
            if (password.Length < 8)
            {
                throw new ArgumentException("Expected at least 8 characters long.", nameof(password));
            }

            using var hmac = new HMACSHA512();
            PasswordSalt = hmac.Key;
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public bool VerifyPasswordHash(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be empty or whitespace.", nameof(password));
            }
            if (PasswordHash.Length != 64)
            {
                throw new ArgumentException("Invalid length of password hash (64 bytes expected).");
            }
            if (PasswordSalt.Length != 128)
            {
                throw new ArgumentException("Invalid length of password salt (128 bytes expected).");
            }

            using var hmac = new HMACSHA512(PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Enumerable.SequenceEqual(computedHash, PasswordHash);
        }

        public void JudgeUserWeight(string Organization)
        {
            if (Organization == null)
            {
                throw new ArgumentException("Invalid Organization.");
            }
            if (Organization == "EAC")
            {
                UserWeight = "2";
            }
        }
        [JsonIgnore]
        public static User AutoJudgeUser
        {
            get
            {
                return new User() { Username = "AutoJudgeUser", Account = "10086" };
            }
        }
        [JsonIgnore]
        public char UserIcon
        {
            get
            {
                return Username.First();
            }
        }
        public override bool Equals(object obj)
        {
            if (obj is User user)
            {
                return user.Username == Username;
            }
            else return false;
        }
    }

    public static class UserDbClass
    {
        private static readonly IMongoCollection<User> Collection = DBconnector.DICSDB.GetCollection<User>("User");
        public static User GetUser(ObjectId id)
        {
            var result = Collection.Find(x => x.Id == id).First();
            return result;
        }
        public static List<User> GetAllUsers()
        {
            var filter = new BsonDocument();
            var result = Collection.Find(filter).ToList();
            return result;
        }
    }
}
