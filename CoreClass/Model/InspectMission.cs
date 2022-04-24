using System;
using System.Collections.Generic;
using System.Linq;
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
        public string PanelID { get; set; }
        public MissionType Type;
        public ObjectId HistoryID;
        public ObjectId ResultContainerId;
        public ProductInfo Info;
        public ObjectId MesLotId;

        public bool Requested = false;
        public bool Finished = false;
        public DateTime LastRequestTime = DateTime.Now;
        public DateTime CreateTime = DateTime.Now;

        public InspectMission(string panelId, MissionType type, ObjectId historyID, ObjectId resultContainerId, ProductInfo info, ObjectId mesLotId)
        {
            PanelID = panelId;
            this.Type = type;
            HistoryID = historyID;
            ResultContainerId = resultContainerId;
            Info = info;
            MesLotId = mesLotId;
        }

        public static void AddInspectMission(InspectMission mission)
        {
            Collection.InsertOneAsync(mission);
        }
        public static KeyValuePair<ProductInfo, int> GetWaittingMissionOverView()
        {
            KeyValuePair<ProductInfo, int> data = new KeyValuePair<ProductInfo, int>();

            var filter = Builders<InspectMission>.Filter.And(
                Builders<InspectMission>.Filter.Eq(x => x.Requested, false));

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
                Builders<InspectMission>.Filter.Eq(x => x.Info.Id, info.Id),
                Builders<InspectMission>.Filter.Eq(x => x.Requested, false));
            var update = Builders<InspectMission>.Update.Set(x => x.LastRequestTime, DateTime.Now).Set(x => x.Requested, false);
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
            var fileter = Builders<InspectMission>.Filter.Eq(x => x.Info.Id, id);
            var mission = Collection.Find(fileter).FirstOrDefault();
            return mission;
        }
        /// <summary>
        /// 用于客户端判级信息发回后刷新任务的状态信息；
        /// </summary>
        /// <param name="inspectMission"></param>
        public static void SetUnfinishedMission(InspectMission inspectMission)
        {
            var filter = Builders<InspectMission>.Filter.Eq(x => x.ID, inspectMission.ID);
            var update = Builders<InspectMission>.Update.Set(x => x.Requested, false);
            Collection.UpdateOneAsync(filter,update);
        }
        /// <summary>
        /// 用于客户端判级信息发回后刷新任务的状态信息；
        /// </summary>
        /// <param name="inspectMission"></param>
        public static void SetFinishedMission(InspectMission inspectMission)
        {
            var filter = Builders<InspectMission>.Filter.Eq(x => x.ID, inspectMission.ID);
            var update = Builders<InspectMission>.Update.Set(x => x.Finished, true);
            Collection.UpdateOneAsync(filter, update);
        }
        /// <summary>
        /// 将超过10min没有返回检查结果的任务进行初始化；
        /// </summary>
        public static void ManageLostMission()
        {
            var time = DateTime.Now - TimeSpan.FromMinutes(15);
            var filter = Builders<InspectMission>.Filter.And(
                Builders<InspectMission>.Filter.Eq(x => x.Finished, false),
                Builders<InspectMission>.Filter.Eq(x => x.Requested, true),
                Builders<InspectMission>.Filter.Lte(x => x.LastRequestTime, time));
            var update = Builders<InspectMission>.Update.Set(x => x.Requested, false);
            Collection.UpdateManyAsync(filter, update);
        }
    }
    /// <summary>
    /// 任务属性标签，用于标记任务的用途，包括正常量产，抽检等；
    /// </summary>
    public enum MissionType
    { 
        MesMission,
        S_GradeCheck,
        F_GradeCheck,
    }
}
