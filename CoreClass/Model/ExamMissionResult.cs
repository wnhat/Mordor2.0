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
    public class ExamMissionResult
    {
        public static IMongoCollection<ExamMissionResult> Collection = DBconnector.DICSDB.GetCollection<ExamMissionResult>("ExamMissionResult");
        [BsonId]
        public ObjectId Id;
        public ObjectId UserId { get; private set; }
        public ObjectId PanelSampleId { get; private set; }

        public DateTime DBInTime = DateTime.Now;

        public DateTime LastModifyTime = DateTime.Now;

        private Defect resultDefect;
        public Defect ResultDefect
        {
            get => resultDefect;
            set 
            {
                resultDefect = value;
                PanelSample panelSample = PanelSample.GetSample(PanelSampleId);
                if (panelSample != null && panelSample.MutiDefect.DefectList.Contains(ResultDefect))
                {
                    IsCorrect = true;
                }
                else IsCorrect = false;
            }
        }
        public bool IsChecked { get; set; }
        public bool IsCorrect { get; private set; }

        public ExamMissionResult(User user,PanelSample panelSample)
        {
            UserId = user.Id;
            PanelSampleId = panelSample.Id;
        }

        public static void AddOne(ExamMissionResult examMissionResult)
        {
            Collection.InsertOneAsync(examMissionResult);
        }
        public static async void AddMany(List<ExamMissionResult> examMissionResult)
        {
            await Collection.InsertManyAsync(examMissionResult);
        }
        /// <summary>
        /// Update value of the specific property;
        /// </summary>
        /// <param name="panelSample"></param>
        /// <param name="porp"></param>
        /// <param name="value"></param>
        public static void UpdateProperty(ExamMissionResult examMissionResult, KeyValuePair<string,object> propValueKeyValuePair)
        {
            var filter = Builders<ExamMissionResult>.Filter.Eq(x => x.Id, examMissionResult.Id);
            var update = Builders<ExamMissionResult>.Update.Set("$LastModifyTime", DateTime.Now).Set(string.Format("${0}", propValueKeyValuePair.Key), propValueKeyValuePair.Value);
            Collection.UpdateOneAsync(filter, update);
        }
        public static void UpdateProperties(ExamMissionResult examMissionResult, List<KeyValuePair<string, object>> propValueKeyValuePairs)
        {
            var filter = Builders<ExamMissionResult>.Filter.Eq(x => x.Id, examMissionResult.Id);
            var update = Builders<ExamMissionResult>.Update.Set(x => x.LastModifyTime, DateTime.Now);
            foreach (KeyValuePair<string, object> item in propValueKeyValuePairs)
            {
                update = update.Set(string.Format("${0}", item.Key), item.Value);
            }
            
            Collection.UpdateOneAsync(filter, update);
        }
    }
}
