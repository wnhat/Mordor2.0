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
        public List<string> LogEvent = new List<string>();
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
        public void AddJudge(OperatorJudge judge, InspectMission inspectMission)
        {
            // 检查任务是否完成；
            var mission = missions.Where(x => x.PanelId == inspectMission.PanelID).FirstOrDefault();
            if (mission == null)
            {
                throw new Exception("该lot中没有这个panel；");
            }
            else
            {
                // 更新内存记录并返回是否完成;
                var finished = mission.AddJudge(judge);

                // update judge to meslot;
                var filter = Builders<MesLot>.Filter.Eq(x => x.ID, this.ID);
                var arrayFilters = new List<ArrayFilterDefinition> { new BsonDocumentArrayFilterDefinition<MesMission>(new BsonDocument("MesMission.PanelId", inspectMission.PanelID)) };
                var option = new UpdateOptions { ArrayFilters = arrayFilters };
                var set = Builders<MesLot>.Update.Set("MesMission.$[MesMission].Judge", mission.Judge);
                LotCollection.UpdateOneAsync(filter, set, option);

                // 当任务需要匹配多人检查结果时，更新inspectmission 数据库信息；
                if (finished)
                {
                    InspectMission.SetFinishedMission(inspectMission);
                    
                    // check if all missions are finished;
                    FinishLot();
                }
                else
                {
                    InspectMission.SetUnfinishedMission(inspectMission);
                }
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
            var filter = builder.And(builder.Eq("CoverTrayId", lot.CoverTrayId), builder.Eq("Update2MES", false));
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
        public void FinishLot()
        {
            // 检查lot是否完成，如果完成，更新lot数据库信息并上传信息；
            if (Finished)
            {
                try
                {
                    MesConnector.FinishInspect(this);
                    SetMeslotFinished();
                }
                catch (System.Exception e)
                {
                    this.AddEvent(e.Message);
                    throw;
                }
            }
        }
        public void AddEvent(string log)
        {
            LogEvent.Add(log);

            var builder = Builders<MesLot>.Filter;
            var filter = builder.And(builder.Eq(x => x.ID, this.ID));
            var update = Builders<MesLot>.Update.Push("LogEvent", log);
        }
        public void SetMeslotFinished()
        {
            var builder = Builders<MesLot>.Filter;
            var filter = builder.And(builder.Eq(x => x.ID, this.ID));
            var update = Builders<MesLot>.Update.Set("Update2MES", true);
            LotCollection.UpdateOneAsync(filter, update);
            //log finish event;
            string logstring = String.Format("检查任务完成，时间为：{0}；", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            AddEvent(logstring);
        }
    }
    public class MesMission
    {
        public InspectMission mission;
        public PanelInspectHistory history;
        public JudgeCore Judge = new JudgeCore();
        public string PanelId;
        public MesMission(string panelid, InspectMission mission, PanelInspectHistory history)
        {
            this.mission = mission;
            this.history = history;
            PanelId = panelid;

            // 用于当任务缺少必要项目时进行记录；
            if (mission == null)
            {
                Judge.AddInspectMissionNullDefect();
            }
            if (history == null)
            {
                Judge.AddHistoryNotFoundDefect();
            }
            if (history.MergeToolJudge == JudgeGrade.E)
            {
                Judge.AddAETJudgeDefect();
            }
        }
        public bool Finished { get { return Judge.finished; } }

        public JudgeGrade LotGrade
        {
            get
            {
                if (Finished)
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
            // 根据N站点的等级及判定结果确定等级
            get
            {
                if (Finished)
                {
                    JudgeGrade DefultJudge = JudgeGrade.W;
                    if (Judge.FinalJudge == JudgeGrade.E)
                    {
                        return DefultJudge;
                    }
                    if (history.MergeToolJudge == JudgeGrade.E)
                    {
                        if (Judge.FinalJudge == JudgeGrade.S)
                        {
                            return DefultJudge;
                        }
                        else if (Judge.FinalJudge == JudgeGrade.E)
                        {
                            return DefultJudge;
                        }
                        else
                        {
                            return JudgeGrade.F;
                        }
                    }
                    else if (history.MergeToolJudge == JudgeGrade.T)
                    {
                        if (Judge.FinalJudge == JudgeGrade.S)
                        {
                            return JudgeGrade.S;
                        }
                        else if (Judge.FinalJudge == JudgeGrade.E)
                        {
                            return DefultJudge;
                        }
                        else
                        {
                            return JudgeGrade.F;
                        }
                    }
                    else if (history.MergeToolJudge == JudgeGrade.Q)
                    {
                        if (Judge.FinalJudge == JudgeGrade.S)
                        {
                            return JudgeGrade.A;
                        }
                        else if (Judge.FinalJudge == JudgeGrade.E)
                        {
                            return DefultJudge;
                        }
                        else
                        {
                            return JudgeGrade.F;
                        }
                    }
                    else if (history.MergeToolJudge == JudgeGrade.D)
                    {
                        if (Judge.FinalJudge == JudgeGrade.S)
                        {
                            return JudgeGrade.W;
                        }
                        else if (Judge.FinalJudge == JudgeGrade.E)
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
                else
                {
                    return JudgeGrade.U;
                }
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
        public JudgeGrade FinalJudge = JudgeGrade.U;
        public Defect FinalDefect;
        public string FinalUsername;
        public string FinalUserId;
        public List<OperatorJudge> judges = new List<OperatorJudge>();
        public static Random GradeRandom = new Random();

        public bool finished { get; internal set; } = false;

        // 新的人员等级判定加入到判定结果中，影响的是结束检查判定结果的概率；
        public bool AddJudge(OperatorJudge NewJudge)
        {
            judges.Add(NewJudge);
            // 当panelscore大于0时，说明人员更倾向于判定为S级
            int PanelScore = 0;

            foreach (var item in judges)
            {
                // If has E judge defect ,then judge Panel to E grade;
                if (item.Defect.DefectName == Defect.OperaterEjudge.DefectName ||
                 item.Defect.DefectName == Defect.HistoryNotFound.DefectName ||
                 item.Defect.DefectName == Defect.InspectMissionNull.DefectName)
                {
                    FinalJudge = JudgeGrade.E;
                    finished = true;
                    FinalDefect = NewJudge.Defect;
                    FinalUsername = User.AutoJudgeUser.Username;
                    FinalUserId = User.AutoJudgeUser.Account;
                    return finished;
                }
                else if (item.Defect == null)
                {
                    PanelScore += item.Score;
                }
                else
                {
                    PanelScore -= item.Score;
                }
            }
            // 第一次添加检查结果，按设定概率进行抽检；
            if (judges.Count == 1)
            {
                if (PanelScore > 0)
                {
                    int Randomscore = GradeRandom.Next(0, 100);
                    if (Randomscore < Parameter.SgradeSimplingRatio)
                    {
                        // keep unfinished;
                        return finished;
                    }
                    else
                    {
                        // finish and set to S grade;
                        FinishJudge(JudgeGrade.S, null, judges[0].Account, judges[0].UserName);
                        return finished;
                    }
                }
                else
                {
                    // finish and set to F grade;
                    FinishJudge(JudgeGrade.F, judges[0].Defect, judges[0].Account, judges[0].UserName);
                    return finished;
                }
            }
            // if have two judge , reinspect;
            else if (judges.Count == 2)
            {
                return finished;
            }
            else
            {
                // if more judges,more chance to finish;
                int Randomscore = GradeRandom.Next(0, 100) * judges.Count;
                // S Grade;
                if (PanelScore > 0)
                {
                    if (Randomscore < Parameter.SgradeSimplingRatio)
                    {
                        // keep unfinished;
                        return finished;
                    }
                    else
                    {
                        // finish and set to S grade;
                        var judge = judges[GradeRandom.Next(0, judges.Count)];
                        FinishJudge(JudgeGrade.S, null, judge.Account, judge.UserName);
                        return finished;
                    }
                }
                // F Grade;
                else
                {
                    if (Randomscore > Parameter.FgradeSimplingRatio)
                    {
                        var highestScorejudge = judges.OrderByDescending(x => x.Score).First();
                        // finish;
                        FinishJudge(JudgeGrade.F, highestScorejudge.Defect, highestScorejudge.Account, highestScorejudge.UserName);
                        return finished;
                    }
                    else
                    {
                        // keep unfinished;
                        return finished;
                    }
                }
            }
        }
        // 任务完成，修改检查结果；
        public void FinishJudge(JudgeGrade grade, Defect defect, string userid, string username)
        {
            FinalJudge = grade;
            FinalDefect = defect;
            FinalUserId = userid;
            FinalUsername = username;
            finished = true;
        }
        // 当N站点历史记录不存在时添加该defect判定作为记录；
        public void AddHistoryNotFoundDefect()
        {
            OperatorJudge judge = new OperatorJudge(Defect.HistoryNotFound, User.AutoJudgeUser.Username, User.AutoJudgeUser.Account, null);
            AddJudge(judge);
        }
        public void AddInspectMissionNullDefect()
        {
            OperatorJudge judge = new OperatorJudge(Defect.InspectMissionNull, User.AutoJudgeUser.Username, User.AutoJudgeUser.Account, null);
            AddJudge(judge);
        }
        internal void AddAETJudgeDefect()
        {
            OperatorJudge judge = new OperatorJudge(Defect.AETEjudge, User.AutoJudgeUser.Username, User.AutoJudgeUser.Account, null);
            AddJudge(judge);
        }
    }
}