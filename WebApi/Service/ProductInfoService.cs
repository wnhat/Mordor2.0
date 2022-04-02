using DICS_WebApi.Models;
using WebApi.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Service
{
    public interface IProductInfoService
    {
        Task<List<ProductInfo>> GetProductInfos();
        Task UploadIMG(ProductInfo product);
        Task AddNewProduct(ProductInfo product);
        Task UpdateProduct(ProductInfo product);
        Task DeleteProduct(ProductInfo product);
    }

    public class ProductInfoService : IProductInfoService
    {
        private readonly IMongoCollection<ProductInfo> _productInfo;

        public ProductInfoService(IUserDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _productInfo = database.GetCollection<ProductInfo>(settings.UsersCollectionName.ProductInfo);
        }

        public Task<List<ProductInfo>> GetProductInfos()
        {
            return Task.Run(() => {
                var filter = Builders<ProductInfo>.Filter.Empty;
                return _productInfo.Find(filter).ToList(); 
            });
        }

        public Task UploadIMG(ProductInfo product)
        {
            return Task.Run(() =>
            {
                product.Img.ImgName = "boe.jpeg";
                string filepath = @"E:\img\" + product.Img.ImgName;
                product.Img.Data = ImgToByte(filepath);

                var filter = Builders<ProductInfo>.Filter.Eq("Name", product.Name);
                var update = Builders<ProductInfo>.Update.Set("Img", product.Img);
                _productInfo.UpdateOne(filter, update);
            });
        }

        public Task AddNewProduct(ProductInfo product)
        {
            return Task.Run(() =>
            {
                var filter = Builders<ProductInfo>.Filter.Eq("Name", product.Name);
                if (_productInfo.Find(filter).FirstOrDefault() != null)
                {
                    throw new ApplicationException("产品已存在");
                }

                _productInfo.InsertOne(product);
            });
        }

        public Task UpdateProduct(ProductInfo product)
        {
            return Task.Run(() =>
            {
                var filter = Builders<ProductInfo>.Filter.Eq("Name", product.Name);
                ProductInfo storeData = _productInfo.Find(filter).FirstOrDefault();

                if (storeData == null)
                {
                    throw new ApplicationException("产品信息不存在");
                }

                product.Id = storeData.Id;
                product.Img.ImgName = storeData.Img.ImgName;
                product.Img.Data = storeData.Img.Data;

                if (CompareProperties(product, storeData))
                {
                    throw new ApplicationException("无信息更新，请重新检查输入");
                }

                _productInfo.ReplaceOne(filter, product);
            });
        }

        public Task DeleteProduct(ProductInfo product)
        {
            return Task.Run(() =>
            {
                var filter = Builders<ProductInfo>.Filter.Eq("Name", product.Name);
                if (_productInfo.Find(filter).FirstOrDefault() == null)
                {
                    throw new ApplicationException("产品信息不存在");
                }

                _productInfo.DeleteOne(filter);
            });
        }

        private static byte[] ImgToByte(string path)
        {
            FileStream fs = new(path, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new(fs);
            byte[] Data = binaryReader.ReadBytes((int)fs.Length);

            return Data;
        }

        private static bool CompareProperties(object obj1, object obj2)
        {
            
            var bsonElements1 = BsonDocument.Create(obj1.ToBsonDocument());
            var bsonElements2 = BsonDocument.Create(obj2.ToBsonDocument());
            return bsonElements1 == bsonElements2;


//            BinaryFormatter binaryFormatter = new();
//            MemoryStream memoryStream1;
//            MemoryStream memoryStream2;
//            using (memoryStream1 = new())
//            {
//#pragma warning disable SYSLIB0011 // obsolete method (Object )
//                binaryFormatter.Serialize(memoryStream1, obj1);
//            }
//            using (memoryStream2 = new())
//            {
//                binaryFormatter.Serialize(memoryStream2, obj2);
//#pragma warning restore SYSLIB0011
//            }
//            string MD5_1 = MD5Encrypt(memoryStream1.ToArray());
//            string MD5_2 = MD5Encrypt(memoryStream2.ToArray());

//            return (MD5_1 == MD5_2);
        }

        private static string MD5Encrypt(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }

            MD5 md5Hasher = MD5.Create();
            byte[] hashBytes = md5Hasher.ComputeHash(bytes);
            string result = BitConverter.ToString(hashBytes);

            return result.Replace("-", "").ToUpper();
        }

    }
}
