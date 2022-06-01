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
        private User? user;

        public UserInfoViewModel()
        {
            
        }
        
        public UserInfoViewModel(User user)
        {
            this.user = user;
        }

        public User User
        {
            get => user;
            set => SetProperty(ref user, value);
        }

        public AuthenticateResult Authenticate(string account, string password)
        {
            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
            {
                return AuthenticateResult.EmptyInput;
            }
            var collection = DBconnector.DICSDB.GetCollection<User>("User");
            var filter = Builders<User>.Filter.Eq("Account", account);
            User = collection.Find(filter).FirstOrDefault();
            if (User == null)
            {
                return AuthenticateResult.AccountNotExist;
            }
            else if (!User.VerifyPasswordHash(password))
            {
                return AuthenticateResult.PasswordError;
            }
            else
            {
                return AuthenticateResult.Success;
            }
        }

        //public bool UserExist()
        //{
        //    if (user != null)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        public bool UserExist
        {
            get => user != null;
        }

        public void Logout()
        {
            user = null;
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

    public enum AuthenticateResult
    {
        EmptyInput,
        AccountNotExist,
        PasswordError,
        Success
    }
}
