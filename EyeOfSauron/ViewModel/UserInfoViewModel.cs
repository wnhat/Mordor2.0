using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using System.Security.Cryptography;
using System.Drawing;
using System.Drawing.Imaging;
using CoreClass;
using CoreClass.Model;

namespace EyeOfSauron.ViewModel
{
    public class UserInfoViewModel : ViewModelBase
    {
        private User _user;

        public UserInfoViewModel()
        {
            _user = new User();
        }

        public User User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        public void Authenticate(string account, string password)
        {
            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
            {
                throw new Exception("Empty input");
            }
            var collection = DBconnector.DICSDB.GetCollection<User>("User");
            var filter = Builders<User>.Filter.Eq("Account", account);
            _user = collection.Find(filter).FirstOrDefault();
            if (_user == null)
            {
                throw new Exception("Account not exist");
            }
            else if (!_user.VerifyPasswordHash(password))
            {
                throw new Exception("Password error");
            }
        }

        public bool UserExist()
        {
            if (_user != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be empty or whitespace.", nameof(password));
            }
            if (storedHash.Length != 64)
            {
                throw new ArgumentException("Invalid length of password hash (64 bytes expected).", nameof(storedHash));
            }
            if (storedSalt.Length != 128)
            {
                throw new ArgumentException("Invalid length of password salt (128 bytes expected).", nameof(storedSalt));
            }
            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Enumerable.SequenceEqual(computedHash, storedHash);
            }
        }
    }
}
