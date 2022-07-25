using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreClass.Model
{
    /// <summary>
    /// Result of every inspect mission and every operator;
    /// </summaryInspectMissionResult
    public class InspectMissionResult
    {
        private static readonly IMongoCollection<InspectMissionResult> Collection = DBconnector.DICSDB.GetCollection<InspectMissionResult>("InspectMissionResult");
        [BsonId]
        public ObjectId id;

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DBInTime { get; }
        public ObjectId InspMissionId { get; }
        public double TactTime { get;}
        public User User { get; private set; }
        public Defect ResultDefect { get; private set; }
        public DicsEqp Eqp { get; set; }
        
        public InspectMissionResult(ObjectId inspMissionId, ObjectId userId, DicsEqp dicsEqp, Defect defect,double tactTime)
        {
            this.InspMissionId = inspMissionId;
            SetUserId(userId);
            Eqp = dicsEqp;
            ResultDefect = defect;
            this.TactTime = tactTime;
            DBInTime = DateTime.Now;
        }
        private void SetUserId(ObjectId value)
        {
            User = UserDbClass.GetUser(value);
        }
        public static void InsertOne(InspectMissionResult inspectMissionResult)
        {
            Collection.InsertOne(inspectMissionResult);
        }
    }
}
