using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using System.Security.Cryptography;
using System.Drawing;
using System.Drawing.Imaging;
using CoreClass;
using CoreClass.Model;

namespace EyeOfSauron
{
    public class UserManager
    {
        User user;
        Bitmap image;
        Image image1;
        public UserManager()
        {
            user = new User();
        }
        public void Authenticate(string account, string password)
        {
            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
            {
                throw new Exception("Empty input");
            }
            var colcetion = DBconnector.DICSDB.GetCollection<User>("User");
            var filter = Builders<User>.Filter.Eq("Account", account);
            user = colcetion.Find(filter).FirstOrDefault();

            //var colcetion1 = DBConnecter.database.GetCollection<AVIResult>("AVIResult");
            //var filter1 = Builders<AVIResult>.Filter.Eq("PanelId", "712B210008B2BAR17");
            //AVIResult aVIResult = colcetion1.Find(filter1).FirstOrDefault();
            //byte[] buffer = aVIResult.DirContainer.DirContainerArray[0].FileContainerArray[0].Data;
            //MemoryStream ms = new MemoryStream(buffer);
            //Image image1 = Image.FromStream(ms);
            //Bitmap image = new Bitmap(ms);
            //BitmapImage image = new BitmapImage();
            //image.BeginInit();
            //image.UriSource = new Uri(@"D:\DICS Software\DefaultSample\AVI\Orign\DefaultSample\00_DUST_CAM00.bmp", UriKind.Absolute);
            //image.EndInit();

            if (user == null)
            {
                throw new Exception("Account not exist");
            }
            else if (!user.VerifyPasswordHash(password))
            {
                throw new Exception("Password error");
            }
        }
        public bool UserExist()
        {
            if (user != null)
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
