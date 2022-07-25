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
    public class ExamMissionCollection
    {
        [BsonIgnore]
        private static readonly IMongoCollection<ExamMissionCollection> Collection = DBconnector.DICSDB.GetCollection<ExamMissionCollection>("ExamMissionWIP");
        [BsonId]
        public ObjectId Id;
        public ObjectId UserID { get; private set; }
        public bool IsFinished { get; set; }

        private string missionCollectionName;
        public string MissionCollectionName 
        {
            get => missionCollectionName;
            set
            {
                missionCollectionName = value;
                MissionCount = PanelSample.GetSampleCount(MissionCollectionName).GetValue("count").AsInt32;
            }
        }
        public int MissionCount { get; private set; }

        public ExamMissionCollection(User user, string MissionCollectionName)
        {
            UserID = user.Id;
            this.MissionCollectionName = MissionCollectionName;
            IsFinished = false;
        }

        public static void AddOne(ExamMissionCollection examMissionWIP)
        {
            Collection.InsertOneAsync(examMissionWIP);
        }

        public static void UpdateOrAdd(ExamMissionCollection examMissionCollection)
        {
            var result = Collection.Find(x => x.UserID == examMissionCollection.Id && x.MissionCollectionName == examMissionCollection.MissionCollectionName && x.IsFinished == false).ToList();
            if(!(result.Count > 0))
            {
                AddOne(examMissionCollection);
            }
        }
        public static List<ExamMissionCollection> GetByUser(ObjectId id)
        {
            var result = Collection.Find(x => x.UserID == id && x.IsFinished == false).ToList();
            return result;
        }

        public static List<BsonDocument> GetUserByCollectionName(string name)
        {
            ProjectionDefinition<ExamMissionCollection> group = "{_id : '$UserID'}";
            var agg = Collection.Aggregate()
                .Match(x => x.IsFinished == false && x.MissionCollectionName == name)
                .Group(group);
            var result = agg?.ToList();
            return result;
        }

        public static void FinishOne(ObjectId id)
        {
            var filter = Builders<ExamMissionCollection>.Filter.Eq(x => x.Id, id);
            var update = Builders<ExamMissionCollection>.Update.Set(x => x.IsFinished, true);
            Collection.UpdateOne(filter, update);
        }
    }
}
