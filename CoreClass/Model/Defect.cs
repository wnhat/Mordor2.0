using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CoreClass.Model
{
    [BsonIgnoreExtraElements]
    public class Defect
    {
        [BsonIgnore]
        [JsonIgnore]
        public static IMongoCollection<Defect> Collection = DBconnector.DICSDB.GetCollection<Defect>("DefectCode");
        [BsonIgnore]
        [JsonIgnore]
        static List<Defect> DefectsList = new List<Defect>();

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
        public int Grade { get; set; }
        [JsonProperty("note", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Note { get; set; }
        // defect judge weight for reinspect or not, when have multiple defects, >1 to increase reinspect probability;
        public decimal weight { get; set; }

        public Defect(string defectName, string defectCode)
        {
            DefectName = defectName;
            DefectCode = defectCode;
        }

        [BsonIgnore]
        [JsonIgnore]
        public static List<Defect> AllDefects
        {
            get
            {
                // get all defects;
                var filter = new BsonDocument();
                var result = Collection.Find(filter).ToList();
                return result;
            }
        }

        //public static Defect GetDefectByCode(string defectcode)
        //{
        //    var filter = Builders<Defect>.Filter.Eq("DefectCode", defectcode);
        //    var defect = Collection.Find(filter).FirstOrDefault();
        //    return defect;
        //}
        //public static Defect GetDefectByName(string defectname)
        //{
        //    var filter = Builders<Defect>.Filter.Eq("DefectName", defectname);
        //    var defect = Collection.Find(filter).FirstOrDefault();
        //    return defect;
        //}
        [JsonIgnore]
        public static Defect HistoryNotFound
        {
            get
            {
                return new Defect("FileNotFound", "DE00000");
            }
        }
        [JsonIgnore]
        public static Defect InspectMissionNull
        {
            get
            {
                return new Defect("MissionInitialFail", "DE00001");
            }
        }
        [JsonIgnore]
        public static Defect OperaterEjudge
        {
            get
            {
                return new Defect("OperaterEjudge", "DE00002");
            }
        }
        [JsonIgnore]
        public static Defect AETEjudge
        {
            get
            {
                return new Defect("AETEjudge", "DE00003");
            }
        }
        [JsonIgnore]
        public static Defect MTPPTNjudge
        {
            get
            {
                return new Defect("MTPPTN", "DE00004");
            }
        }
        // get defect by defect code;
        public static Defect GetDefectByCode(string defectCode)
        {
            return DefectsList.Find(x => x.DefectCode == defectCode);
        }
        // get defect by defect name;
        public static Defect GetDefectByName(string defectName)
        {
            return DefectsList.Find(x => x.DefectName == defectName);
        }
        //refresh the defect list from database;
        public static void RefreshDefectList()
        {
            DefectsList = new List<Defect>();
            DefectsList = Defect.AllDefects;
        }
        public override string ToString()
        {
            return DefectName;
        }
    }
}