﻿using CoreClass;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;
using CoreClass.Service;
using CoreClass.DICSEnum;
using MongoDB.Bson.Serialization.Attributes;
using CoreClass.Model;
using NetMQ;
using CoreClass.LogSpider;
using System.Threading.Tasks;
using System.Linq;
using System.Xml;
using Serilog;
using System.Net.Http;

namespace Mordor2._0
{
    class Program
    {
        //public static ILogger Testlogger = new LoggerConfiguration()
        //        .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose)
        //        .CreateLogger();

        //static string connectstring = "mongodb://172.16.200.100:27017";
        //static MongoClient mongoClient = new MongoClient(connectstring);
        ///// <summary>
        ///// DICS 生产数据库
        ///// </summary>
        //public static IMongoDatabase DICSDB = mongoClient.GetDatabase("DICSAuto");
        //static IMongoCollection<PC> IP = DICSDB.GetCollection<PC>("IP");
        //static IMongoCollection<PanelInspectHistory> Result = DICSDB.GetCollection<PanelInspectHistory>("InspectResult");
        //static IMongoCollection<ProductInfo> ProductInfoCollection = DBconnector.DICSDB.GetCollection<ProductInfo>("ProductInfo");
        ///// <summary>
        ///// DICS 测试数据库
        ///// </summary>
        //static IMongoDatabase DICS = mongoClient.GetDatabase("DICS");

        static void Main()
        {
            string[] ids = new string[] { "713D1X0030A4ABJ11", "713D1X0030A4ABJ11" };
            var a = BuildXjudge(ids, "7CTCT31");
        }
        static void Test()
        {
            string ip = "10.141.34.78";
            int port = 28108;
            string part = "EAC";
            var builder = new UriBuilder(Uri.UriSchemeHttp, ip, port, part);
            
            Uri uri = builder.Uri;
            var aa = uri.ToString();

            var a = new CutServerConnector();
            DateTime start = DateTime.Parse("2022-06-17 00:20:01");
            DateTime end = DateTime.Parse("2022-06-17 18:10:00");
            var bb = a.GetInfo(DateTime.Now - TimeSpan.FromDays(8),DateTime.Now);
        }
        public static string BuildXjudge(string[] ids, string eqp)
        {
            string machineName = eqp;
            string tranID = DateTime.Now.ToString(@"yyyyMMddHHmmssffffff");
            XmlDocument xmlDoc = new XmlDocument();

            //Message节点
            XmlElement mesElement = xmlDoc.CreateElement("Message");

            //Header节点 & 子节点添加
            XmlElement headElement = xmlDoc.CreateElement("Header");
            XmlElement[] HeadChildElements = new XmlElement[9];
            HeadChildElements[0] = xmlDoc.CreateElement("MESSAGENAME");
            HeadChildElements[0].InnerText = "AOIPanelJudgeReport";
            HeadChildElements[1] = xmlDoc.CreateElement("SHOPNAME");
            HeadChildElements[1].InnerText = "EAC";
            HeadChildElements[2] = xmlDoc.CreateElement("TRANSACTIONID");
            HeadChildElements[2].InnerText = tranID;
            HeadChildElements[3] = xmlDoc.CreateElement("ORIGINALSOURCESUBJECTNAME");
            HeadChildElements[3].InnerText = "BOE.B7.CUT.AOI." + machineName;
            HeadChildElements[4] = xmlDoc.CreateElement("SOURCESUBJECTNAME");
            HeadChildElements[4].InnerText = "BOE.B7.CUT.AOI." + machineName;
            HeadChildElements[5] = xmlDoc.CreateElement("TARGETSUBJECTNAME");
            HeadChildElements[5].InnerText = "BOE.B7.MEM.PRD.PEMsvr";
            HeadChildElements[6] = xmlDoc.CreateElement("EVENTUSER");
            HeadChildElements[6].InnerText = machineName;
            HeadChildElements[7] = xmlDoc.CreateElement("EVENTCOMMENT");
            HeadChildElements[7].InnerText = "AOIPanelJudgeReport";
            HeadChildElements[8] = xmlDoc.CreateElement("listener");
            HeadChildElements[8].InnerText = "PEMListener";
            foreach (var item in HeadChildElements) headElement.AppendChild(item);
            mesElement.AppendChild(headElement);

            //Body节点 & 子节点添加
            XmlElement bodyElement = xmlDoc.CreateElement("Body");
            XmlElement[] bodyChildElements = new XmlElement[5];
            bodyChildElements[0] = xmlDoc.CreateElement("MACHINENAME");
            bodyChildElements[0].InnerText = machineName;
            bodyChildElements[1] = xmlDoc.CreateElement("PROCESSOPERATIONNAME");
            bodyChildElements[1].InnerText = "C20000N";
            bodyChildElements[2] = xmlDoc.CreateElement("PRODUCTSPECNAME");
            bodyChildElements[2].InnerText = "";
            bodyChildElements[3] = xmlDoc.CreateElement("PRODUCTIONTYPE");
            //panelList
            bodyChildElements[4] = xmlDoc.CreateElement("PANELLIST");
            foreach (var item in ids)
            {
                XmlElement panelElement = xmlDoc.CreateElement("PANEL");
                XmlElement panel_name_el = xmlDoc.CreateElement("PANELNAME");
                panel_name_el.InnerText = item;
                XmlElement judge_type_el = xmlDoc.CreateElement("JUDGETYPE");
                judge_type_el.InnerText = "LLOAOI";
                XmlElement judge_el = xmlDoc.CreateElement("JUDGE");
                judge_el.InnerText = "X";
                panelElement.AppendChild(panel_name_el);
                panelElement.AppendChild(judge_type_el);
                panelElement.AppendChild(judge_el);
                bodyChildElements[4].AppendChild(panelElement);
            }
            foreach (var item in bodyChildElements) bodyElement.AppendChild(item);
            mesElement.AppendChild(bodyElement);
            xmlDoc.AppendChild(mesElement);
            //保存为字符串
            MemoryStream stream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
            xmlDoc.Save(writer);
            StreamReader sr = new StreamReader(stream, System.Text.Encoding.UTF8);
            stream.Position = 0;
            string result = sr.ReadToEnd();
            sr.Close();
            stream.Close();
            return result;
        }
    }
}
