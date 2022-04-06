using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace CoreClass.Model
{
    [BsonIgnoreExtraElements]
    public class Defect
    {
        [BsonIgnore]
        [JsonIgnore]
        public static IMongoCollection<Defect> Collection = DBconnector.DICSDB.GetCollection<Defect>("DefectCode");

        [BsonId]
        [JsonProperty("id")]
        public ObjectId Id { get; set; }
        [JsonProperty("code")]
        public string DefectCode { get; set; }
        [JsonProperty("name")]
        public string DefectName { get; set; }
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

        public Defect(string defectName, string defectCode)
        {
            DefectName = defectName;
            DefectCode = defectCode;
        }
        public override string ToString()
        {
            return DefectName;
        }
        public static Defect GetDefectByCode(string defectcode)
        {
            var filter = Builders<Defect>.Filter.Eq("DefectCode", defectcode);
            var defect = Collection.Find(filter).FirstOrDefault();
            return defect;
        }
        public static Defect GetDefectByName(string defectname)
        {
            var filter = Builders<Defect>.Filter.Eq("DefectName", defectname);
            var defect = Collection.Find(filter).FirstOrDefault();
            return defect;
        }
        [JsonIgnore]
        public Defect HistoryNotFound
        {
            get
            {
                return new Defect("FileNotFound", "DE00000");
            }
        }
        [JsonIgnore]
        public Defect InspectMissionNull
        {
            get
            {
                return new Defect("MissionInitialFail", "DE00001");
            }
        }
    }
}