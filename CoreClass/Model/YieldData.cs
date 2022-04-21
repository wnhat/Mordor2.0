using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization.Attributes;
using CoreClass.DICSEnum;

namespace CoreClass.Model
{
    [BsonIgnoreExtraElements]
    public class YieldData
    {
        [BsonId]
        [JsonProperty("id")]
        public ObjectId Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("producttype")]
        public ProductType OnInspectTypes { get; set; }
        [JsonProperty("yieldtrend")]
        public float[] YieldTrend { get; set; }
        [JsonProperty("average")]
        public float Average { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var bsonElements1 = BsonDocument.Create(this.ToBsonDocument());
            var bsonElements2 = BsonDocument.Create(obj.ToBsonDocument());
            return bsonElements1 == bsonElements2;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
