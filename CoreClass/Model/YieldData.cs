using MongoDB.Bson;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization.Attributes;
using CoreClass.DICSEnum;
using System;

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
        public YieldDataSpecifiedByDay[] YieldTrend { get; set; }
        [JsonProperty("time")]
        public DateTime Time { get; set; }
        [JsonProperty("average")]
        public Double Average { get; set; }
        // 上班次良率
        [JsonProperty("lastshift")]
        public Double LastShift { get; set; }
        // 本班次良率
        [JsonProperty("thisshift")]
        public Double ThisShift { get; set; }

        public class YieldDataSpecifiedByDay

        {
            [JsonConstructor]
            public YieldDataSpecifiedByDay() { }

            public string Date { get; set; }
            public Double Data { get; set; }
        }

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
