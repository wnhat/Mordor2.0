using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreClass.DICSEnum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace CoreClass.Model
{
    public class PanelSample
    {
        public static IMongoCollection<PanelSample> Collection = DBconnector.DICSDB.GetCollection<PanelSample>("SampleMission");
        [BsonId]
        public ObjectId ID;

        [BsonRepresentation(BsonType.String)]
        public JudgeGrade SampleGrade { get; set; }

        [BsonRepresentation(BsonType.String)]
        public MissionType MissionType { get; set; }

        public DateTime DBInTime = DateTime.Now;

        public DateTime LastModifyTime = DateTime.Now;

        public bool IsDeleted { get; set; }

        public string PanelID { get; set; }

        public string MissionCollection { get; set; }

        public string Note { get; set; }

        public AETresult AetResult { get; set; }

        public Sample_MutiDefect MutiDefect { get; set; }

        public PanelSample(AETresult aetResult, string missionCollection, string note, MissionType missionType, Defect[] defects = null)
        {
            PanelID = aetResult.PanelId;
            AetResult = aetResult;
            MissionCollection = missionCollection;
            Note = note;
            MissionType = missionType;
            if(defects == null)
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
        public static async Task<List<BsonDocument>> GetMissionCollection()
        {
            ProjectionDefinition<PanelSample> group = "{collection : '$MissionCollection'";
            var agg = Collection.Aggregate()
                .Match(x => x.IsDeleted == false)
                .Group(group)
                .Sort("{collection: 1 }");
            var result = await agg.ToListAsync();
            return result;
        }

        public static async Task<List<PanelSample>> GetSamples()
        {
            var result = await Collection.Find(x => x.IsDeleted == false).ToListAsync();
            return result;
        }

        /// <summary>
        /// Set delete flag to True;
        /// </summary>
        /// <param name="panelSample"></param>
        public static void PanelSampleDelete(PanelSample panelSample)
        {
            var filter = Builders<PanelSample>.Filter.Eq(x => x.ID, panelSample.ID);
            var update = Builders<PanelSample>.Update.Set(x => x.LastModifyTime, DateTime.Now).Set(x => x.IsDeleted, true);
            Collection.UpdateOneAsync(filter, update);
        }

        /// <summary>
        /// Update value of the specific property;
        /// </summary>
        /// <param name="panelSample"></param>
        /// <param name="porp"></param>
        /// <param name="value"></param>
        public static void UpdateProperty(PanelSample panelSample, string porpName, object value)
        {
            var filter = Builders<PanelSample>.Filter.Eq(x => x.ID, panelSample.ID);
            var update = Builders<PanelSample>.Update.Set(x => x.LastModifyTime, DateTime.Now).Set(string.Format("${0}",porpName), value);
            Collection.UpdateOneAsync(filter, update);
        }

        /// <summary>
        /// Delete the panelSample Permanently;
        /// </summary>
        /// <param name="panelSample"></param>
        public static void PanelSampleDeletePermanently(PanelSample panelSample)
        {
            var filter = Builders<PanelSample>.Filter.Eq(x => x.ID, panelSample.ID);
            Collection.DeleteOneAsync(filter);
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
                string DefectNames = "";
                foreach (Defect defect in DefectList)
                {
                    DefectNames = string.Format("{0};{1}", DefectNames, defect.DefectName);
                }
                return DefectNames;
            }
        }
    }
}
