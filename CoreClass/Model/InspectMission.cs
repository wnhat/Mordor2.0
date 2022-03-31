using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace CoreClass.Model
{
    /// <summary>
    /// 作业员检查任务，添加的任务可能含有多种目的如正常产品的量产，或产品自动的过漏检抽检相关内容；
    /// </summary>
    public class InspectMission
    {
        public static IMongoCollection<InspectMission> Collection = DBconnector.DICSDB.GetCollection<InspectMission>("InspectMission");
        [BsonId]
        public ObjectId ID;
        public string PanelId;
        public MissionType type;
        public ObjectId HistoryID;
        public ObjectId ResultContainerId;
        public ProductInfo Info;
        public ObjectId ParentStorage;
        public ObjectId MesLotId;

        public bool requested = false;
        public DateTime LastRequestTime = DateTime.Now;
        public DateTime CreateTime = DateTime.Now;

        public InspectMission(string panelId, MissionType type, ObjectId historyID, ObjectId resultContainerId, ProductInfo info)
        {
            PanelId = panelId;
            this.type = type;
            HistoryID = historyID;
            ResultContainerId = resultContainerId;
            Info = info;
            // TODO: 检验result的完整性；
        }

        public static void AddInspectMission(InspectMission mission)
        {
            Collection.InsertOneAsync(mission);
        }
        public static KeyValuePair<ProductInfo, int> GetWaittingMissionOverView()
        {
            KeyValuePair<ProductInfo, int> data = new KeyValuePair<ProductInfo, int>();

            var filter = Builders<InspectMission>.Filter.And(
                Builders<InspectMission>.Filter.Eq(x => x.requested, false));

            var projection = new ProjectionDefinitionBuilder<InspectMission>().Include(x => x.Info);

            var result = Collection.Aggregate()
                .Match(filter)
                .Group(projection);
            return data;
        }
        /// <summary>
        /// 用于客户端查询数据库中待检查的任务；
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static InspectMission GetMission(ProductInfo info)
        {
            var filter = Builders<InspectMission>.Filter.And(
                Builders<InspectMission>.Filter.Eq(x => x.Info.id, info.id),
                Builders<InspectMission>.Filter.Eq(x => x.requested, false));
            var update = Builders<InspectMission>.Update.Set(x => x.LastRequestTime, DateTime.Now).Set(x => x.requested, true);

            InspectMission mission = Collection.FindOneAndUpdate(filter, update);
            return mission;
        }
        /// <summary>
        /// 用于查询特定inspectmission；
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static InspectMission GetMission(ObjectId id)
        {
            var fileter = Builders<InspectMission>.Filter.Eq(x => x.Info.id, id);
            var mission = Collection.Find(fileter).FirstOrDefault();
            return mission;
        }
    }
    /// <summary>
    /// 任务属性标签
    /// </summary>
    public enum MissionType
    { 
        MesMission,
        S_GradeCheck,
        F_GradeCheck,
    }
}
