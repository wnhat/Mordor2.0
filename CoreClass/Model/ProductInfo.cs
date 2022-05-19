using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using CoreClass.DICSEnum;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using MongoDB.Driver;

namespace CoreClass.Model
{
    [BsonIgnoreExtraElements]
    public class ProductInfo
    {
        // mongodb product info collection;
        static IMongoCollection<ProductInfo> Collection = DBconnector.DICSDB.GetCollection<ProductInfo>("ProductInfo");

        [BsonId]
        public ObjectId Id { get; set; }
        public string PrefixId { get; set; } //761L;
        [JsonProperty("name")]
        public string Name { get; set; }    //D2 Porto;
        public string[] InspectImageNames { get; set; }
        [BsonRepresentation(BsonType.String)]
        [JsonProperty("producttype")]
        //[JsonConverter(typeof(StringEnumConverter))]
        public ProductType[] OnInspectTypes { get; set; }
        [JsonProperty("fgcode")]
        public string FGcode { get; set; }
        [JsonProperty("modelid")]
        public string ModelId { get; set; }
        public DateTime LastAddTime { get; set; }
        [JsonProperty("img")]
        public ImageContainer Img { get; set; }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return Id.Equals(((ProductInfo)obj).Id);
        }
        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        public override string ToString()
        {
            return base.ToString();
        }
        /// <summary>
        /// 仅测试使用
        /// </summary>
        /// <returns></returns>
        public static ProductInfo GetProductInfo()
        {
            // random
            Random rnd = new Random();
            var find = Collection.Find(new BsonDocument()).ToList();
            // get the first product in mongodb;
            var count = Collection.CountDocuments(new BsonDocument());
            var randomint = rnd.Next(0, (int)count);
            return find[randomint];
        }
    }
}
