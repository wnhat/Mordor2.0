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

        public ExamMissionWIP(User user, string MissionCollectionID)
        {
            Id = user.Id;
            this.MissionCollectionName = MissionCollectionID;
        }
        public static void AddOne(ExamMissionWIP examMissionWIP)
        {
            Collection.InsertOneAsync(examMissionWIP);
        }
        public static async void AddMany(List<ExamMissionWIP> examMissionWIPs)
        {
            await Collection.InsertManyAsync(examMissionWIPs);
        }
        public static async Task<List<ExamMissionWIP>> GetByUser(ObjectId id)
        {
            var result = await Collection.Find(x => x.UserID == id).ToListAsync();
            return result;
        }

        public static List<BsonDocument> GetUserByCollectionName(string name)
        {
            ProjectionDefinition<ExamMissionWIP> group = "{_id : 'UserID'}";
            var agg = Collection.Aggregate()
                .Match(x => x.MissionCollectionName == name)
                .Group(group);
            var result = agg?.ToList();
            return result;
        }

        public static void DeleteOne(ObjectId userId, string missionCollectionID)
        {
            Collection.DeleteOne(x => x.UserID == userId && x.MissionCollectionName == missionCollectionID);
        }
    }
}
