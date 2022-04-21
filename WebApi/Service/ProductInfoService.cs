using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using CoreClass.Model;
using CoreClass;

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
        private static readonly IMongoCollection<ProductInfo> _productInfo = DBconnector.DICSDB.GetCollection<ProductInfo>("ProductInfo");

        public Task<List<ProductInfo>> GetProductInfos()
        {
            return Task.Run(() => {
                var filter = Builders<ProductInfo>.Filter.Empty;
                var products = _productInfo.Find(filter).ToList();
                return products; 
            });
        }

        public Task UploadIMG(ProductInfo product)
        {
            return Task.Run(() =>
            {
                product.Img.Name = "boe.jpeg";
                string filepath = @"E:\img\" + product.Img.Name;
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

                product.LastAddTime = DateTime.Now;
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

                var obj1 = new
                {
                    product.Name,
                    product.OnInspectTypes,
                    product.FGcode,
                    product.ModelId
                };

                var obj2 = new
                {
                    storeData.Name,
                    storeData.OnInspectTypes,
                    storeData.FGcode,
                    storeData.ModelId
                };

                if (CompareProperties(obj1, obj2))
                {
                    throw new ApplicationException("无信息更新，请重新检查输入");
                }

                var update = Builders<ProductInfo>.Update.Set("Name", product.Name)
                                                         .Set("OnInspectTypes", product.OnInspectTypes)
                                                         .Set("FGcode", product.FGcode)
                                                         .Set("ModelId", product.ModelId)
                                                         .Set("LastAddTime", DateTime.Now);
                _productInfo.UpdateOne(filter, update);
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
        }

    }
}
