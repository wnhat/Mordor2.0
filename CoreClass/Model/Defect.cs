using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace CoreClass.Model
{
    [BsonIgnoreExtraElements]
    public class Defect
    {
        public static IMongoCollection<Defect> Collection = DBconnector.DICSDB.GetCollection<Defect>("DefectCode");

        public string DefectName;
        public string DefectCode;
        public Defect(string defectName, string defectCode)
        {
            DefectName = defectName;
            DefectCode = defectCode;
        }
        public override string ToString()
        {
            return DefectName;
        }
        public static Defect GetDefectByCode(string defectcode)
        {
            var filter = Builders<Defect>.Filter.Eq("DefectCode", defectcode);
            var defect = Collection.Find(filter).FirstOrDefault();
            return defect;
        }
        public static Defect GetDefectByName(string defectname)
        {
            var filter = Builders<Defect>.Filter.Eq("DefectName", defectname);
            var defect = Collection.Find(filter).FirstOrDefault();
            return defect;
        }
        public static Defect HistoryNotFound
        {
            get
            {
                return new Defect("FileNotFound","DE00000");
            }
        }
        public static Defect InspectMissionNull
        {
            get
            {
                return new Defect("MissionInitialFail","DE00001");
            }
        }
    }
}