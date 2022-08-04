using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreClass.DICSEnum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace CoreClass.Model
{
    public class ExamMissionResult
    {
        public static IMongoCollection<ExamMissionResult> Collection = DBconnector.DICSDB.GetCollection<ExamMissionResult>("ExamMissionResult");
        [BsonId]
        public ObjectId Id;
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime dbInTime = DateTime.Now;
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime lastModifyTime = DateTime.Now;
        private Defect resultDefect;
        public ObjectId PanelSampleId { get; private set; }
        public ExamMissionCollection ExamMissionCollection { get; private set; }
        public double TactTime { get; private set; }
        public DicsEqp Eqp { get; private set; }
        public Defect ResultDefect
        {
            get => resultDefect;
            set 
            {
                resultDefect = value;
                PanelSample panelSample = PanelSample.GetSample(PanelSampleId);
                if(panelSample != null)
                {
                    if(panelSample.MutiDefect == null)
                    {
                        IsCorrect = resultDefect == null;
                    }
                    else
                    {
                        IsCorrect = panelSample.MutiDefect.DefectList.Contains(ResultDefect);
                    }
                }
                else IsCorrect = false;
            }
        }
        public bool IsChecked { get; set; }
        public bool IsCorrect { get; private set; }

        public ExamMissionResult(ExamMissionCollection examMissionCollection, ObjectId panelSampleId)
        {
            ExamMissionCollection = examMissionCollection;
            PanelSampleId = panelSampleId;

        }
        public void SetResult(Defect resultDefect, DicsEqp dicsEqp, double tactTime)
        {
            ResultDefect = resultDefect;
            Eqp = dicsEqp;
            TactTime = tactTime;
            IsChecked = true;
        }
        public static void AddOne(ExamMissionResult examMissionResult)
        {
            Collection.InsertOneAsync(examMissionResult);
        }
        public static async Task AddMany(List<ExamMissionResult> examMissionResult)
        {
            await Collection.InsertManyAsync(examMissionResult);
        }

        public static ExamMissionResult GetOneAndUpdate(ExamMissionCollection examMissionCollection)
        {
            var filter = Builders<ExamMissionResult>.Filter.And(
                Builders<ExamMissionResult>.Filter.Eq(x => x.ExamMissionCollection, examMissionCollection),
                Builders<ExamMissionResult>.Filter.Eq(x => x.IsChecked, false));
            var update = Builders<ExamMissionResult>.Update.Set(x => x.lastModifyTime, DateTime.Now).Set(x => x.IsChecked, true);
            ExamMissionResult mission = Collection.FindOneAndUpdate(filter, update);
            //return mission;
            //Get one radnomly;
            var randomOne = Collection.AsQueryable().Where(x => x.ExamMissionCollection == examMissionCollection && x.IsChecked == false).Sample(1).FirstOrDefault();
            if (randomOne == null)
            {
                return null;
            }
            else
            {
                var filter1 = Builders<ExamMissionResult>.Filter.Eq(x => x.Id, randomOne.Id);
                var update1 = Builders<ExamMissionResult>.Update.Set(x => x.lastModifyTime, DateTime.Now).Set(x => x.IsChecked, true);
                ExamMissionResult missionResult = Collection.FindOneAndUpdate(filter, update);
                return missionResult;
            }
        }
        
        /// <summary>
        /// Update value of the specific property;
        /// </summary>
        /// <param name="panelSample"></param>
        /// <param name="porp"></param>
        /// <param name="value"></param>
        public static void UpdateProperty(ObjectId Id, KeyValuePair<string,object> propValueKeyValuePair)
        {
            var filter = Builders<ExamMissionResult>.Filter.Eq(x => x.Id, Id);
            var update = Builders<ExamMissionResult>.Update.Set("$LastModifyTime", DateTime.Now).Set(string.Format("${0}", propValueKeyValuePair.Key), propValueKeyValuePair.Value);
            Collection.UpdateOneAsync(filter, update);
        }

        public static void UpdateProperties(ObjectId Id, List<KeyValuePair<string, object>> propValueKeyValuePairs)
        {
            var filter = Builders<ExamMissionResult>.Filter.Eq(x => x.Id, Id);
            var update = Builders<ExamMissionResult>.Update.Set(x => x.lastModifyTime, DateTime.Now);
            foreach (KeyValuePair<string, object> item in propValueKeyValuePairs)
            {
                update = update.Set(item.Key, item.Value);
            }

            Collection.UpdateOneAsync(filter, update);
        }

        public static async Task<BsonDocument> GetRemainMissionCount(ExamMissionCollection examMissionCollection)
        {
            ProjectionDefinition<ExamMissionResult> group = "{_id : '$Info', count : {$sum : 1}}";
            var result = Collection.Aggregate()
                .Match(x => x.ExamMissionCollection == examMissionCollection && x.IsChecked == false)
                .Group(group);
            return await result.FirstOrDefaultAsync();
        }

        public static List<BsonDocument> GetAccuracyValue(ObjectId id)
        {
            var filter = Builders<ExamMissionResult>.Filter.And(
                Builders<ExamMissionResult>.Filter.Eq(x => x.ExamMissionCollection.Id, id),
                Builders<ExamMissionResult>.Filter.Eq(x => x.IsChecked, true));

            ProjectionDefinition<ExamMissionResult> group = "{_id : '$IsCorrect',count : {$sum : 1}}";
            var agg = Collection.Aggregate()
                .Match(filter)
                .Group(group)
                .Sort("{_id: 1 }");
            var result = agg.ToList();
            return result;
        }
    }
}
