using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CoreClass.DICSEnum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace CoreClass.Model
{
    public class PanelSample
    {
        [BsonIgnore]
        [JsonIgnore]
        public static readonly IMongoCollection<PanelSample> Collection = DBconnector.DICSDB.GetCollection<PanelSample>("SampleMission");
        [BsonId]
        public ObjectId Id;

        [BsonRepresentation(BsonType.String)]
        public JudgeGrade SampleGrade { get; set; }

        [BsonRepresentation(BsonType.String)]

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DBInTime = DateTime.Now;
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastModifyTime = DateTime.Now;

        public bool IsDeleted { get; set; }

        public string PanelID { get; set; }

        public MissionCollection MissionCollection { get; set; }

        public string Note { get; set; }

        public ProductInfo ProductInfo { get; set; }

        public AETresult AetResult { get; set; }

        public Sample_MutiDefect MutiDefect { get; set; }

        public PanelSample(AETresult aetResult, MissionCollection missionCollection, string note, ProductInfo info, List<Defect> defects = null)
        {
            PanelID = aetResult.PanelId;
            AetResult = aetResult;
            ProductInfo = info;
            MissionCollection = missionCollection;
            Note = note;
            if(defects == null || defects.Count == 0)
            {
                SampleGrade = JudgeGrade.S;
            }
            else
            {
                MutiDefect = new(defects);
                SampleGrade = JudgeGrade.F;
            }
        }

        public static void AddOnePanelSample(PanelSample panelSample)
        {
            Collection.InsertOneAsync(panelSample);
        }

        public static async void AddManyPanelMission(List<PanelSample> panelSamples)
        {
            await Collection.InsertManyAsync(panelSamples);
        }

        /// <summary>
        /// Get all collection name of MissionCollection;
        /// </summary>
        /// <returns>List of collection name on <BsonDocument> type</returns>
        public static List<BsonDocument> GetMissionCountCollectionByType(MissionType missionType)
        {
            var filter1 = Builders<PanelSample>.Filter.Eq(x => x.IsDeleted, false);
            var filter2 = Builders<PanelSample>.Filter.Eq(x => x.MissionCollection.MissionType, missionType);
            var filter = Builders<PanelSample>.Filter.And(filter1, filter2);
            return GetMissionCountCollection(filter);
        }
        public static List<BsonDocument> GetMissionCountCollection(FilterDefinition<PanelSample> filter)
        {
            ProjectionDefinition<PanelSample> group = "{_id : '$MissionCollection',count : {$sum : 1}}";
            var agg = Collection.Aggregate()
                .Match(filter)
                .Group(group)
                .Sort("{_id: 1 }");
            var result = agg?.ToList();
            return result;
        }
        public static async Task<List<BsonDocument>> GetMissionCountCollection()
        {
            ProjectionDefinition<PanelSample> group = "{_id : '$MissionCollection', count : {$sum : 1}}";
            var agg = Collection.Aggregate()
                .Match(x => x.IsDeleted == false)
                .Group(group)
                .Sort("{_id: -1 }");
            var result = await agg.ToListAsync();
            return result;
        }

        public static PanelSample GetSample(ObjectId id)
        {
            var result = Collection.Find(x => x.Id == id).First();
            return result;
        }

        public static async Task<List<PanelSample>> GetSamples()
        {
            var result = await Collection.Find(x => x.IsDeleted == false).ToListAsync();
            return result;
        }

        public static async Task<List<PanelSample>> GetSamples(string collectionName)
        {
            var result = await Collection.Find(x => x.IsDeleted == false && x.MissionCollection.CollectionName == collectionName).ToListAsync();
            return result;
        }

        public static List<BsonDocument> GetSampleIds(string collectionName)
        {
            ProjectionDefinition<PanelSample> group = "{_id : '$_id'}";
            var agg = Collection.Aggregate()
                .Match(x => x.IsDeleted == false && x.MissionCollection.CollectionName == collectionName)
                .Group(group);
            var result = agg.ToList();
            return result;
        }

        public static BsonDocument GetSampleCount(string collectionName)
        {
            ProjectionDefinition<PanelSample> group = "{_id : '$MissionCollection', count : {$sum : 1}}";
            var agg = Collection.Aggregate()
                .Match(x => x.IsDeleted == false && x.MissionCollection.CollectionName == collectionName)
                .Group(group)
                .Sort("{ count: -1 }");
            var result = agg.First();
            return result;
        }


        /// <summary>
        /// Set delete flag to True;
        /// </summary>
        /// <param name="panelSample"></param>
        public static async void PanelSampleDelete(ObjectId Id)
        {
            var filter = Builders<PanelSample>.Filter.Eq(x => x.Id, Id);
            var update = Builders<PanelSample>.Update.Set(x => x.LastModifyTime, DateTime.Now).Set(x => x.IsDeleted, true);
            await Collection.UpdateOneAsync(filter, update);
        }

        /// <summary>
        /// Update value of the specific property;
        /// </summary>
        /// <param name="panelSample"></param>
        /// <param name="porp"></param>
        /// <param name="value"></param>
        public static async void UpdateProperty(ObjectId Id, string porpName, object value)
        {
            await UpdateProperty("_id", Id, porpName, value);
        }
        public static async Task UpdateProperty(string filterPorpName, object eqValue, string porpName, object value)
        {
            var filter = Builders<PanelSample>.Filter.Eq(filterPorpName, eqValue);
            var update = Builders<PanelSample>.Update.Set(x => x.LastModifyTime, DateTime.Now).Set(porpName, value);
            await Collection.UpdateManyAsync(filter, update);
        }

        /// <summary>
        /// Delete the panelSample Permanently;
        /// </summary>
        /// <param name="panelSample"></param>
        public static void PanelSampleDeletePermanently(PanelSample panelSample)
        {
            var filter = Builders<PanelSample>.Filter.Eq(x => x.Id, panelSample.Id);
            Collection.DeleteOneAsync(filter);
        }
    }
    public class MissionCollection
    {
        public string CollectionName { get; set; }
        [BsonRepresentation(BsonType.String)]
        public MissionType MissionType { get; set; }
        public MissionCollection()
        {

        }
        public MissionCollection(string name, MissionType missionType = MissionType.Sample)
        {
            CollectionName = name;
            MissionType = missionType;
        }
    }
    public class Sample_MutiDefect
    {
        public List<Defect> DefectList = new();
        public Sample_MutiDefect(Defect[] defects) 
        {
            for(int i = 0; i < defects.Length; i++)
            {
                DefectList.Add(defects[i]);
            }
        }
        public Sample_MutiDefect(List<Defect> defects)
        {
            this.DefectList = defects;
        }
        public string DefectCode
        {
            get 
            {
                string DefectCodes = "";
                foreach (Defect defect in DefectList)
                {
                    DefectCodes = string.Format("{0};{1}", DefectCodes, defect.DefectCode);
                }
                return DefectCodes;
            }
        }
        public string DefectName 
        {
            get
            {
                string DefectNames = DefectList.FirstOrDefault().DefectName;
                for(int i = 1; i < DefectList.Count; i++)
                {
                    DefectNames = string.Format("{0}；{1}", DefectNames, DefectList[i].DefectName);
                }
                return DefectNames;
            }
        }
    }
}
