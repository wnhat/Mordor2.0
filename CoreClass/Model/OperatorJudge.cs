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
        public int Score { get; set; }
        // todo: 注意引用时null值问题；
        public ObjectId? UserId { get; set; }
        public string UserName { get; set; }
        public string Account { get; set; }
        public OperatorJudge(Defect defect, string userName, string account, ObjectId? userId, int score)
        {
            this.UserName = userName;
            this.Account = account;
            this.Defect = defect;
            UserId = userId;
            Score = score;
        }
    }
}
