using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DICS_WebApi.Models
{
    [BsonIgnoreExtraElements]
    public class DefectCode
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("group1")]
        public string Group1 { get; set; }
        [JsonProperty("group2", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Group2 { get; set; }
        [JsonProperty("group3", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Group3 { get; set; }
        [JsonProperty("grade")]
        public int Grade { get; set; } = 5!;
        [JsonProperty("note", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Note { get; set; }
    }
}
