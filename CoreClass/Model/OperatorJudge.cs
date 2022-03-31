using CoreClass.DICSEnum;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreClass.Model
{
    public class OperatorJudge
    {
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public Defect Defect { get; set; }
        // Only S,E,F grade;
        public JudgeGrade judge { get; set; }
        public InspectMission InspectMission { get; set; }
        public string PanelID { get; set; }
        public int Score { get; set; }
        public ObjectId UserId { get; set; }
        public string UserName { get; set; }
        public string UserNumber { get; set; }
        public OperatorJudge(User user, InspectMission mission, Defect defect, JudgeGrade judge)
        {
            this.PanelID = mission.PanelId;
            this.UserId = user.Id;
            this.UserName = UserName;
            this.UserNumber = user.UserNumber;
            this.Defect = defect;
            this.judge = judge;
        }
        public MissionType MissionType { get { return InspectMission.type; } }
        // 当inspectmission 的requesttime 和相应的judge不同时，该judge结果将被抛弃；
        public DateTime RequestTime { get { return InspectMission.LastRequestTime; } }
    }
}
