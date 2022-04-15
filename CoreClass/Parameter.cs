using CoreClass.Model;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CoreClass;
using MongoDB.Driver;

namespace CoreClass
{
    // 使用mongodb中的parameter表储存设置信息，使用该软件的电脑应能够连接至服务器主机；
    public static class Parameter
    {
        public static string SavePath;
        public static string[] AviImageNameList;
        public static string[] SviImageNameList;
        public static string[] AppImageNameList;
        public static int PreLoadQuantity;
        public static Defect[] CodeNameList;
        public static string AviExamFilePath;
        public static string SviExamFilePath;
        public static int MesConnectTimeOut;
        // 抽样比例为0~100 内的整数；
        public static int SgradeSimplingRatio;
        public static int FgradeSimplingRatio;

        static Parameter()
        {
            // Get parameter from mongodb;
            var Collection = DBconnector.DICSDB.GetCollection<BsonDocument>("Parameter");
            var Filter = new BsonDocument();
            // find new parameter by date;
            var result = Collection.Find(Filter).SortByDescending(x => x["_id"]).FirstOrDefault();
            result.RemoveElement(result.GetElement("_id"));
            JObject jsonobj = JObject.Parse(result.ToJson());
            var fieldcollection = typeof(Parameter).GetFields();
            if (CompareNameList(fieldcollection, jsonobj))
            {
                foreach (var item in fieldcollection)
                {
                    var propertyName = item.Name;
                    var value = jsonobj.GetValue(propertyName);
                    Type propertytype = item.FieldType;
                    var convertvalue = value.ToObject(propertytype);
                    item.SetValue(null, convertvalue);
                }
            }
        }
        static private bool CompareNameList(FieldInfo[] fieldcollection, JObject jsonobj)
        {
            // 对比Parameter属性列表与 json 文件中的差异（）版本；
            List<string> fieldnamelist = new List<string>();
            List<string> jsonnamelist = new List<string>();
            foreach (var item in fieldcollection)
            {
                fieldnamelist.Add(item.Name);
            }
            foreach (var item in jsonobj)
            {
                jsonnamelist.Add(item.Key);
            }
            if (fieldnamelist.Except(jsonnamelist).Count() != 0)
            {
                throw new ApplicationException("系统参数多于文件记录，请检查版本对应关系");
            }
            else if (jsonnamelist.Except(fieldnamelist).Count() != 0)
            {
                throw new ApplicationException("文件记录多于系统参数，请检查版本对应关系");
            }
            else
            {
                return true;
            }
        }
        public static void Save()
        {

        }
    }
}