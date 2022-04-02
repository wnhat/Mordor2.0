using CoreClass;
using CoreClass.DICSEnum;
using CoreClass.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sauron
{
    public class MesLot
    {
        public static IMongoCollection<MesLot> LotCollection = DBconnector.DICSDB.GetCollection<MesLot>("MesLot");

        [BsonId]
        public ObjectId ID;
        public string CoverTrayId;
        public string[] Panels;
        public string Xmlstring;
        public bool Added = false;
        public bool Update2MES = false;
        public DateTime CreateTime = DateTime.Now;
        public ProductInfo ProductInfo;
        public ProductType ProductType;
        public List<MesMission> missions = new List<MesMission>();
        public MesLot(string coverTrayId, string[] panels, string xmlstring, ProductType productType, ProductInfo productInfo)
        {
            CoverTrayId = coverTrayId;
            Panels = panels;
            Xmlstring = xmlstring;
            this.ProductType = productType;
            this.ProductInfo = productInfo;
            // 对ID进行排序方便校对重复添加的任务；
            Array.Sort(Panels);
        }
        public bool Finished
        {
            get
            {
                foreach (var item in missions)
                {
                    if (!item.Finished)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public bool Contains(ObjectId inspectmissionid)
        {
            foreach (var mission in missions)
            {
                if (inspectmissionid == mission.mission.ID)
                {
                    return true;
                }
            }
            return false;
        }
        public bool AddJudge(OperatorJudge judge)
        {
            // 检查任务是否完成；
            var mission = missions.Where(x => x.PanelId == judge.PanelID).FirstOrDefault();
            if (mission == null)
            {
                throw new Exception("该lot中没有这个panel；");
            }
            else
            {
                // 更新数据库记录;
                var filter = Builders<MesLot>.Filter.Eq(x => x.ID, this.ID);
                var update = Builders<MesLot>.Update.Push<OperatorJudge>("MesMission.$[MesMission].Judge.judges", judge);
                var arrayFilters = new List<ArrayFilterDefinition> { new BsonDocumentArrayFilterDefinition<MesMission>(new BsonDocument("MesMission.PanelId", judge.PanelID)) };
                var option = new UpdateOptions { ArrayFilters = arrayFilters };
                LotCollection.UpdateOneAsync(filter, update, option);
                // 更新内存记录并返回是否完成;
                return mission.AddJudge(judge);
            }
        }
        /// <summary>
        /// 向mongodb中添加新的lot，但该lot并没有进行初始化（仅包含从mes获取到的信息）；
        /// </summary>
        /// <param name="lot"></param>
        public static void Insert(MesLot lot)
        {
            // todo: 校验是否有在检lot被重复添加的情况；
            var builder = Builders<MesLot>.Filter;
            var filter = builder.And(builder.Eq("CoverTrayId", lot.CoverTrayId),builder.Eq("Update2MES", false));
            var result = LotCollection.Find(filter).FirstOrDefault();
            if (result == null)
            {
                LotCollection.InsertOneAsync(lot);
            }
            else
            {
                var panelidEnum = result.Panels.GetEnumerator();
                // 当 lot 中的panelid全部相同时当作同一lot处理；
                int equalCount = 0;
                for (int i = 0; i < result.Panels.Length; i++)
                {
                    if (lot.Panels[i] == result.Panels[i])
                    {
                        equalCount++;
                    }
                }
                if (equalCount == result.Panels.Length)
                {
                    // do nothing;
                }
                else
                {
                    // TODO: 记录该可能存在异常的情况；
                    // 当lot中panel信息不完全相同时添加lot，仅能避免作业员操作lot勿解hold的情况,无法解决被人工操作的lot MES 信息与DICS信息不匹配的问题；
                    LotCollection.InsertOneAsync(lot);
                }
            }
        }
    }
    public class MesMission
    {
        public InspectMission mission;
        public PanelInspectHistory history;
        public JudgeCore Judge = new JudgeCore();
        public string PanelId;
        public MesMission(InspectMission mission, PanelInspectHistory history)
        {
            this.mission = mission;
            this.history = history;
            PanelId = history.PanelId;
            if (history != null && mission != null)
            {
                Judge.LastGrade = history.LastJudge;
            }
            else
            {
                // 用于当任务缺少必要项目时进行记录；
                if (mission == null)
                {
                    Judge.AddInspectMissionNullDefect();
                }
                if (history == null)
                {
                    Judge.AddHistoryNotFoundDefect();
                }
            }
        }
        public bool Finished { get { return Judge.finished; } }
        public JudgeGrade LotGrade
        {
            get
            {
                return Judge.LotGrade;
            }
        }
        public JudgeGrade PanelJudge
        {
            // 根据N站点的等级及判定结果确定等级
            get
            {
                return Judge.FinalJudge;
            }
        }
        public bool AddJudge(OperatorJudge judge)
        {
            var finished = Judge.AddJudge(judge);
            return finished;
        }
    }
    public class JudgeCore
    {
        public JudgeGrade LastGrade;
        public List<OperatorJudge> judges = new List<OperatorJudge>();
        public static Random FgradeRandom = new Random();
        public static Random SgradeRandom = new Random();

        public bool finished { get; internal set; } = false;

        public JudgeGrade LotGrade
        {
            get
            {
                if (finished)
                {
                    if (FinalJudge == JudgeGrade.S)
                    {
                        return JudgeGrade.G;
                    }
                    else
                    {
                        return JudgeGrade.N;
                    }
                }
                else
                {
                    return JudgeGrade.U;
                }
            }
        }
        public JudgeGrade FinalJudge
        {
            get
            {
                JudgeGrade DefultJudge = JudgeGrade.W;
                if (Judge.LastGrade == JudgeGrade.E)
                {
                    return DefultJudge;
                }
                if (history.LastJudge == JudgeGrade.E)
                {
                    if (Judge.LastGrade == JudgeGrade.S)
                    {
                        return DefultJudge;
                    }
                    else if (Judge.LastGrade == JudgeGrade.E)
                    {
                        return DefultJudge;
                    }
                    else
                    {
                        return JudgeGrade.F;
                    }
                }
                else if (history.LastJudge == JudgeGrade.T)
                {
                    if (Judge.LastGrade == JudgeGrade.S)
                    {
                        return JudgeGrade.S;
                    }
                    else if (Judge.LastGrade == JudgeGrade.E)
                    {
                        return DefultJudge;
                    }
                    else
                    {
                        return JudgeGrade.F;
                    }
                }
                else if (history.LastJudge == JudgeGrade.Q)
                {
                    if (Judge.LastGrade == JudgeGrade.S)
                    {
                        return JudgeGrade.A;
                    }
                    else if (Judge.LastGrade == JudgeGrade.E)
                    {
                        return DefultJudge;
                    }
                    else
                    {
                        return JudgeGrade.F;
                    }
                }
                else if (history.LastJudge == JudgeGrade.D)
                {
                    if (Judge.LastGrade == JudgeGrade.S)
                    {
                        return JudgeGrade.W;
                    }
                    else if (Judge.LastGrade == JudgeGrade.E)
                    {
                        return DefultJudge;
                    }
                    else
                    {
                        return JudgeGrade.F;
                    }
                }
                else
                {
                    return DefultJudge;
                }
            }
        }
        public bool AddJudge(OperatorJudge judge)
        {
            judges.Add(judge);

            return finished;
        }
        // 当N站点历史记录不存在时添加该defect判定作为记录；
        public void AddHistoryNotFoundDefect()
        {
            judges.Add(Defect.);
        }
        public void AddInspectMissionNullDefect()
        {

        }
    }

    [Serializable]
    public class JudgeUnsuitbleException : Exception
    {
        public JudgeUnsuitbleException() { }
        public JudgeUnsuitbleException(string message) : base(message) { }
        public JudgeUnsuitbleException(string message, Exception inner) : base(message, inner) { }
        protected JudgeUnsuitbleException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}