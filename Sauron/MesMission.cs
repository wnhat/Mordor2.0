using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreClass.Model;
using MongoDB.Bson;

namespace Sauron
{
    //public class MesMission
    //{
    //    public ObjectId ParentMesLotID;
    //    public InspectMission mission;
    //    public PanelInspectHistory history;
    //    public JudgeCore Judge = new JudgeCore();
    //    public string PanelId;
    //    public MesMission(ObjectId parentMesLotID, InspectMission mission, PanelInspectHistory history)
    //    {
    //        ParentMesLotID = parentMesLotID;
    //        this.mission = mission;
    //        this.history = history;
    //        PanelId = history.PanelId;
    //    }
    //    public bool Finished { get { return Judge.finished; } }
    //    public bool AddJudge(OperatorJudge judge)
    //    {
    //        var finished = Judge.AddJudge(judge);
    //        return finished;
    //    }
    //}
}
