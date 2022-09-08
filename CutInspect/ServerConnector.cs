﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using TIBCO.Rendezvous;

namespace CutInspect
{
    public static class ServerConnector
    {
        static RestClient restClient = new RestClient("http://10.141.34.78:28108/EAC/");
        static string service = "21200";
        static string network = ";225.21.21.2";
        static string daemon = "10.141.70.61:7500";
        static string subject = "BOE.B7.MEM.PRD.PEMsvr";
        static Transport transport = null;

        static ServerConnector()
        {
            restClient.Options.MaxTimeout = 10000;

            // initial MES connector
            try
            {
                TIBCO.Rendezvous.Environment.Open();
            }
            catch (RendezvousException exception)
            {
                Console.Error.WriteLine("Failed to open Rendezvous Environment: {0}", exception.Message);
                Console.Error.WriteLine(exception.StackTrace);
                System.Environment.Exit(1);
            }

            // Create Network transport
            try
            {
                transport = new NetTransport(service, network, daemon);
            }
            catch (RendezvousException exception)
            {
                Console.Error.WriteLine("Failed to create NetTransport");
                Console.Error.WriteLine(exception.StackTrace);
                System.Environment.Exit(1);
            }
        }
        public static InspectItem[] GetInfo(DateTime starttime,DateTime endtime)
        {            
            var request = new RestRequest("getImageInfo");

            request.AddQueryParameter("startTime", starttime.ToString("yyyy-MM-dd HH:mm:ss"));
            request.AddQueryParameter("endTime", endtime.ToString("yyyy-MM-dd HH:mm:ss"));

            var response = restClient.Get(request);
            if (response.IsSuccessful)
            {
                if (response.Content == null)
                {
                    throw new Exception("获取的产品信息为空；");
                }
                else
                {
                    string jsonstring = response.Content;
                    InspectItem[] deserializedProduct = JsonConvert.DeserializeObject<InspectItem[]>(jsonstring);
                    return deserializedProduct;
                }
            }
            else
            {
                throw new Exception("连接失败；");
            }
        }
        public static List<GroupData> GetGroupedData(InspectItem[] data)
        {
            List<GroupData> groupedData = new List<GroupData>();
            var eqplist = from item in data
                          group item.equipmentId by item.equipmentId into g
                          select new { equipmentId = g.Key, Count = g.Count() };

            foreach (var eq in eqplist)
            {
                var items = from item in data where item.equipmentId == eq.equipmentId select item;

                var newgroup = new GroupData(eq.equipmentId, items.ToList());
                groupedData.Add(newgroup);
            }
            return groupedData;
        }
        public static void SendResult(string id,int status)
        {
            var request = new RestRequest("modifyImageStatus",Method.Post);
            var jsonresult = new JObject();
            jsonresult["id"] = id;
            jsonresult["status"] = status;

            request.AddJsonBody(jsonresult.ToString());
            var response = restClient.Post(request);
            if (response.IsSuccessful)
            {
            }
            else
            {
                throw new Exception("上传检查结果失败，请检查服务器连接；");
            }
        }
        public static Stream GetImage(string id)
        {
            var request = new RestRequest("getImage");

            request.AddQueryParameter("imageId", id);

            var response = restClient.Get(request);
            if (response.IsSuccessful)
            {
                if (response.RawBytes == null)
                {
                    throw new Exception("获取的图像为空；");
                }
                else
                {
                return new MemoryStream(response.RawBytes);
                }
            }
            else
            {
                throw new Exception("连接失败；");
            }
        }
        public static void SendXjudge(string[] ids)
        {
            Message message = new Message();
            try
            {
                message.SendSubject = subject;
            }
            catch (RendezvousException exception)
            {
                // todo: log
            }
            try
            {
                // todo: log 
                string str = BuildXjudge(ids);
                message.AddField(new MessageField("xmlData", str));
                transport.Send(message);
            }
            catch (RendezvousException exception)
            {
                // todo: log
            }
        }
        public static string BuildXjudge(string[] ids)
        {
            string machineName = "ImgInsp";
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

            return xmlDoc.InnerXml;
        }
    }
}
