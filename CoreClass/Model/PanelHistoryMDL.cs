using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;

namespace CoreClass.Model
{
    public class PanelHistoryMDL
    {
        public static IMongoCollection<PanelHistoryMDL> Collection = DBconnector.DICSDB.GetCollection<PanelHistoryMDL>("MDLInspectResult");

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [JsonIgnore]
        public ObjectId ID { get; set; }
        public string PanelId { get; set; }
        public string EqName { get; set; }
        // 为东八区本地时间；
        public DateTime InspectTime { get; set; }
        public long InspectTimeInt { get; set; }
        public DateTime DbIntime { get; set; } = DateTime.Now;
        public string OperatorID { get; set; }
        public string FGcode { get; set; }
        public string ProductType { get; set; }
        // 模组设备在检查这张屏时所用的recipe名字，但不知道是点灯recipe名字还是检查recipe名字；
        public string RecipeName { get; set; }
        public string PanelJudge { get; set; }
        public string PanelGrade { get; set; }
        // 站点信息 例 C52000N
        public string ProcessName { get; set; }
        public string Defect { get; set; }
        public PanelHistoryMDL(XmlDocument xmlDocument)
        {
            EqName = xmlDocument.GetElementsByTagName("MACHINENAME")[0].InnerText;
            OperatorID = xmlDocument.GetElementsByTagName("OPERATORID")[0].InnerText;
            PanelId = xmlDocument.GetElementsByTagName("PANELNAME")[0].InnerText;
            FGcode = xmlDocument.GetElementsByTagName("PRODUCTSPECNAME")[0].InnerText;
            ProductType = xmlDocument.GetElementsByTagName("PRODUCTIONTYPE")[0].InnerText;
            RecipeName = xmlDocument.GetElementsByTagName("MACHINERECIPENAME")[0].InnerText;
            PanelJudge = xmlDocument.GetElementsByTagName("PANELJUDGE")[0].InnerText;
            PanelGrade = xmlDocument.GetElementsByTagName("PANELGRADE")[0].InnerText;
            ProcessName = xmlDocument.GetElementsByTagName("PROCESSOPERATIONNAME")[0].InnerText;

            InspectTime = FormateDatetime(xmlDocument.GetElementsByTagName("TRANSACTIONID")[0].InnerText);
            InspectTimeInt = InspectTime.ToUniversalTime().Ticks;
            var defects = xmlDocument.GetElementsByTagName("DEFECTCODE");
            Defect = "";
            for (int i = 0; i < defects.Count; i++)
            {
                Defect += defects[i].InnerText;
                if (i != 1 && i != defects.Count)
                {
                    Defect += ",";
                }
            }
        }
        static DateTime FormateDatetime(string time)
        {
            // 20221103133341000741
            var year = int.Parse(time.Substring(0, 4));
            var month = int.Parse(time.Substring(4, 2));
            var day = int.Parse(time.Substring(6, 2));
            var hour = int.Parse(time.Substring(8, 2));
            var minute = int.Parse(time.Substring(10, 2));
            var second = int.Parse(time.Substring(12, 2));
            DateTime newtime = new DateTime(year, month, day, hour, minute, second,DateTimeKind.Local);
            return newtime;
        }
        public static string Serialize(PanelHistoryMDL obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            //BsonDocument buffer = new BsonDocument();
            //var writer = new BsonDocumentWriter(buffer);
            //BsonSerializer.Serialize<PanelHistoryMDL>(writer, obj);
            //var json = buffer.ToJson(new JsonWriterSettings() {OutputMode = JsonOutputMode.CanonicalExtendedJson });
            return json;
        }
        public static PanelHistoryMDL Deserialize(string json)
        {
            return BsonSerializer.Deserialize<PanelHistoryMDL>(json);
        }
    }
}
