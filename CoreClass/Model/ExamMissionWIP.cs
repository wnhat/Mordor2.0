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
    public class ExamMissionWIP
    {
        public static IMongoCollection<ExamMissionWIP> Collection = DBconnector.DICSDB.GetCollection<ExamMissionWIP>("ExamMissionWIP");
        [BsonId]
        public ObjectId Id;
        public ObjectId UserID { get; private set; }

        private string missionCollectionId;
        public string MissionCollectionId 
        {
            get => missionCollectionId;
            set
            {
                missionCollectionId = value;
                //MissionCount = PanelSample.GetSamples(MissionCollectionId).Result.Count;
                MissionCount = PanelSample.GetSampleCount(MissionCollectionId);
            }
        }
        public int MissionCount { get; private set; }

        public ExamMissionWIP(User user, string MissionCollectionID)
        {
            Id = user.Id;
            this.MissionCollectionId = MissionCollectionID;
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
        public static void DeleteOne(ObjectId userId, string missionCollectionID)
        {
            Collection.DeleteOne(x => x.UserID == userId && x.MissionCollectionId == missionCollectionID);
        }
    }
}
