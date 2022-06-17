using System;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using System.Security.Cryptography;
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

        public bool UserExist
        {
            get => !(user == null || user.Equals(User.AutoJudgeUser));
        }

        public void Logout()
        {
            //Incase view binding error,show something after logedout;
            User = User.AutoJudgeUser;
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
