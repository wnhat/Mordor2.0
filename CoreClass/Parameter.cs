using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreClass
{
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
        //public static DateTime SearchAfterDate = DateTime.Parse("2022-01-10T09:30:41");

        static Parameter()
        {
            string sysConfigPath = @"D:\DICS Software\sysconfig.json";
            FileInfo sysconfig = new FileInfo(sysConfigPath);
            if (sysconfig.Exists)
            {
                var jsonreader = new StreamReader(sysconfig.OpenRead());
                var jsonstring = jsonreader.ReadToEnd();
                JObject jsonobj = JObject.Parse(jsonstring);
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
            else
            {
                throw new ApplicationException("sysconfig.json 文件不存在，请检查与 145.22电脑的链接或相应地址是否存在设置文件；");
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
    public class Defect
    {
        public string DefectName;
        public string DefectCode;
        public Defect(string defectName, string defectCode)
        {
            DefectName = defectName;
            DefectCode = defectCode;
        }
        public Defect() { }
        public override string ToString()
        {
            return DefectName;
        }
    }
}
