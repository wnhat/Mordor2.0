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
    public class ExamMissionWIP
    {
        [BsonIgnore]
        private static readonly IMongoCollection<ExamMissionWIP> Collection = DBconnector.DICSDB.GetCollection<ExamMissionWIP>("ExamMissionWIP");
        [BsonId]
        public ObjectId Id;
        public ObjectId UserID { get; private set; }

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

        public ExamMissionWIP(User user, string MissionCollectionName)
        {
            UserID = user.Id;
            this.MissionCollectionName = MissionCollectionName;
        }
        public static void AddOne(ExamMissionWIP examMissionWIP)
        {
            Collection.InsertOneAsync(examMissionWIP);
        }
        public static async void AddMany(List<ExamMissionWIP> examMissionWIPs)
        {
            await Collection.InsertManyAsync(examMissionWIPs);
        }

        public static List<ExamMissionWIP> GetByUser(ObjectId id)
        {
            var result = Collection.Find(x => x.UserID == id).ToList();
            return result;
        }

        public static List<BsonDocument> GetUserByCollectionName(string name)
        {
            ProjectionDefinition<ExamMissionWIP> group = "{_id : '$UserID'}";
            var agg = Collection.Aggregate()
                .Match(x => x.MissionCollectionName == name)
                .Group(group);
            var result = agg?.ToList();
            return result;
        }

        public static void DeleteOne(ObjectId userId, string missionCollectionName)
        {
            if (missionCollectionName != null)
            {
                Collection.DeleteOne(x => x.UserID == userId && x.MissionCollectionName == missionCollectionName);
            }
        }
        public static void DelectMany(string missionCollectionName)
        {
            if(missionCollectionName != null)
            {
                Collection.DeleteMany(x => x.MissionCollectionName == missionCollectionName);
            }
        }
    }
}
