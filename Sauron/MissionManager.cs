using CoreClass;
using CoreClass.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sauron
{
    public class MissionManager
    {

        static IMongoCollection<ProductInfo> ProductInfoCollection = DBconnector.DICSDB.GetCollection<ProductInfo>("ProductInfo");

        public void FinishMission(OperatorJudge judge, InspectMission mission)
        {
            if (mission.Type == MissionType.MesMission)
            {
                MesMissionStorage.AddJudge(judge, mission);
            }
            else if (mission.Type == MissionType.S_GradeCheck)
            {

            }
        }
        /// <summary>
        /// 自动循环向Mes搜索待检查E级任务，直到该型号没有待添加任务跳出循环；
        /// </summary>
        public void AddAutoMission()
        {
            // TODO:Delay时间使用数据库更新内容；Now default value is 120 min;
            TimeSpan AddTimeDelay = TimeSpan.FromMinutes(120);
            var products = ProductInfoCollection.Find(new BsonDocument()).ToList();
            var MissionInitialTask = Task.Delay(60000).ContinueWith(taskdelay => InitialMesMission());
            foreach (var product in products)
            {
                if (product.OnInspectTypes.Count() > 0 && DateTime.Now - product.LastAddTime > AddTimeDelay)
                {
                    foreach (var productType in product.OnInspectTypes)
                    {
                        // 循环至STK中所有符合条件的Lot被添加；
                        while (true)
                        {
                            try
                            {
                                MesLot newmissionlot = MesConnector.RequestMission(product, productType);
                                if (newmissionlot == null)
                                {
                                    break;
                                }
                                else
                                {
                                    // 向数据库添加新的lot信息（仅包含mes信息，需要对lot进行初始化操作才能添加检查项目）;
                                    MesLot.Insert(newmissionlot);
                                }
                            }
                            catch (MesMessageException E)
                            {
                                Log.Logger.Error("发生了mes消息异常：", E);
                                break;
                            }
                            catch (Exception E)
                            {
                                Log.Logger.Error("在请求MES任务时发生了未经处理的异常情况；", E);
                                break;
                            }
                        }
                    }
                }
            }
        }
        public void InitialMesMission()
        {
            var builder = Builders<MesLot>.Filter;
            var filter = builder.Eq(x => x.Added, false);
            var addlots = MesLot.LotCollection.Find(filter).ToEnumerable();
            foreach (var lot in addlots)
            {
                try
                {
                    MesMissionStorage.InitialLot(lot);
                }
                catch (Exception e)
                {
                    Log.Logger.Error(e, "初始化lot时发生了未经处理的异常；");
                }

            }
        }
        /// <summary>
        /// 向任务集中添加抽检任务;
        /// </summary>
        public void AddCheckMission()
        {

        }
        /// <summary>
        /// 生成测试任务不进行mes通信；通过mongodb 近时段产生的结果生成随机lot；
        /// </summary>
        public void AddMissionTest()
        {
            var builder = Builders<AETresult>.Filter;
            // get top 12 panel from mongodb collection "AETresult", order by time;
            var result = AETresult.AETresultCollection.Find(new BsonDocument()).SortByDescending(x => x.history.InspDate).Limit(12).ToList();

            string[] panels = new string[result.Count];
            for (int i = 0; i < result.Count; i++)
            {
                panels[i] = result[i].PanelId;
            }
            MesLot newmissionlot = new MesLot("TESTLOT", panels, "", CoreClass.DICSEnum.ProductType.Production, ProductInfo.GetProductInfo());
            MesLot.Insert(newmissionlot);
            InitialMesMission();
        }
    }
    public static class MesMissionStorage
    {
        // 键为 lotid
        public static Dictionary<ObjectId, MesLot> LotContainer = new Dictionary<ObjectId, MesLot>();

        static MesMissionStorage()
        {

        }
        public static void InitialLot(MesLot lot)
        {
            string[] panels = lot.Panels;
            // 为lot初始化每个panel的任务；
            foreach (var panel in panels)
            {
                MesMission newmission = CreateMesMission(panel, lot.ProductInfo, lot.ID);
                if (!newmission.Finished)
                {
                    // 为初始化完成但为自动判定的任务创建检查数据；
                    InspectMission.AddInspectMission(newmission.mission);
                }
                lot.missions.Add(newmission);
            }
            var filter = Builders<MesLot>.Filter.Eq(x => x.ID, lot.ID);
            var update = Builders<MesLot>.Update.Set(x => x.missions, lot.missions).Set(x => x.Added, true);
            MesLot.LotCollection.UpdateOneAsync(filter, update);
        }
        public static MesMission CreateMesMission(string panelid, ProductInfo info, ObjectId lotid)
        {
            PanelInspectHistory history = PanelInspectHistory.Get(panelid);
            // 根据historyid 查找对应的检查结果文件；
            AETresult result = AETresult.Get(history.ID);
            if (result == null)
            {
                return new MesMission(panelid, null, history);
            }
            else
            {
                InspectMission mission = new InspectMission(panelid, MissionType.MesMission, history.ID, result.Id, info, lotid);
                return new MesMission(panelid, mission, history);
            }
        }
        public static void AddJudge(OperatorJudge judge, InspectMission Inspectmission)
        {
            var lotid = Inspectmission.MesLotId;
            var mission = InspectMission.GetMission(Inspectmission.ID);
            if (LotContainer.ContainsKey(lotid) && Inspectmission.LastRequestTime == mission.LastRequestTime && mission != null)
            {
                LotContainer[lotid].AddJudge(judge, Inspectmission);
            }
            else
            {
                Log.Logger.Information("客户端发回的检查结果被抛弃{@Judge}", judge);
            }
        }
    }
}