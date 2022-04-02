using CoreClass;
using CoreClass.DICSEnum;
using CoreClass.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using TIBCO.Rendezvous;

namespace Sauron
{
    public class MesConnector
    {
        string service = "21200";
        string network = ";225.21.21.2";
        string daemon = "10.141.70.61:7500";
        string subject = "BOE.B7.MEM.PRD.PEMsvr";
        Transport transport = null;
        public MesConnector()
        {
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
        //public MesLot RequestMissionTest()
        //{
        //    //TODO: addtest；
        //}
        public MesLot RequestMission(ProductInfo info, ProductType type)
        {
            string fgCode = info.FGcode;
            string productInfo = type.ToString();

            RemoteTrayGroupInfoDownloadRequest newrequest = new RemoteTrayGroupInfoDownloadRequest(fgCode, productInfo);
            LogMesMessage(newrequest.GetXmlDocument());

            Message newmessage = newrequest.GetMessage();
            newmessage.SendSubject = subject;

            Message reply = null;
            try
            {
                reply = transport.SendRequest(newmessage, Parameter.MesConnectTimeOut);
            }
            catch (RendezvousException e)
            {
                Log.MesLog.Error("向MES请求产品 {0} {1} 时发生曾经令程序崩溃的异常，请调查原因", fgCode, productInfo);
                string errorstring = e.Message;
                throw new MesMessageException(errorstring);
            }
            // TIBCO 超时未接到返回信息时，返回值为null；
            if (reply == null)
            {
                string errorstring = String.Format("向MES请求任务 {0} {1} 超时，请检查与MES的连接或网络问题；", fgCode, productInfo);
                Log.MesLog.Error(errorstring);
                throw new MesMessageException(errorstring);
            }
            else
            {
                string xml = reply.GetField("xmlData").Value.ToString();

                XmlDocument returnxml = new XmlDocument();
                returnxml.LoadXml(xml);

                MesMessageType messagetype = (MesMessageType)Enum.Parse(typeof(MesMessageType), returnxml.GetElementsByTagName("MESSAGENAME")[0].InnerText);
                if (messagetype == MesMessageType.OpCallSend)
                {
                    string errorstring = returnxml.GetElementsByTagName("OPCALLDESCRIPTION")[0].InnerText;
                    //当返回信息为无剩余任务时返回Null；
                    if (errorstring.Contains("can not be found"))
                    {
                        return null;
                    }
                    Log.MesLog.Error("向MES请求任务时发生错误，错误信息为：{0}", errorstring);
                    throw new MesMessageException(errorstring);
                }
                else
                {
                    LogMesMessage(xml);

                    var coverName = returnxml.GetElementsByTagName("TRAYGROUPNAME")[0].InnerText;
                    var nodelist = returnxml.GetElementsByTagName("PANELID");
                    if (nodelist.Count == 0)
                    {
                        throw new MesMessageException("该返回消息中没有Panel的信息，请检查Mes消息的完整性；");
                    }
                    else
                    {
                        List<string> newpanellist = new List<string>();
                        foreach (var item in nodelist)
                        {
                            string panelid = ((XmlNode)item).InnerText;
                            if (newpanellist.Contains(panelid))
                            {
                                throw new MesMessageException("该返回消息包含了重复的panel id，请检查Mes消息的正确性；");
                            }
                            else
                            {
                                newpanellist.Add(panelid);
                            }
                        }
                        return new MesLot(coverName, newpanellist.ToArray(), xml, type, info);
                    }
                }
            }
        }
        public void FinishInspect(MesLot lot)
        {
            RemoteTrayGroupProcessEnd newfinished = new RemoteTrayGroupProcessEnd(lot);
            LogMesMessage(newfinished.GetXmlDocument());
            Message newfinishedMessage = newfinished.GetMessage();
            newfinishedMessage.SendSubject = subject;
            var reply = transport.SendRequest(newfinishedMessage, Parameter.MesConnectTimeOut);
            // 超时未接到返回信息时，返回值为null；
            if (reply == null)
            {
                string errorstring = "向MES发送已完成任务超时，请检查与MES的连接或网络问题；";
                //MesLogClass.Logger.Error(errorstring);
                string logfile = @"D:\Mordor\LOG\MesLog";
                logfile = System.IO.Path.Combine(logfile, DateTime.Now.ToShortDateString(), lot.CoverTrayId);
                newfinished.Save(logfile);
                throw new MesMessageException(errorstring);
            }
            else
            {
                var newrep = reply.GetFieldByIndex(0);
                string xml;
                if (newrep.Name == "xmlData")
                {
                    xml = reply.GetField("xmlData").Value.ToString();
                }
                else
                {
                    xml = newrep.Value.ToString();
                }
                LogMesMessage(xml);
                XmlDocument returnxml = new XmlDocument();
                returnxml.LoadXml(xml); //MessageField可隐式转换为xmldocument；

                RemoteTrayGroupProcessEndReply returnmessage = new RemoteTrayGroupProcessEndReply(returnxml);
                if (returnmessage.Result == true)
                {
                    Log.MesLog.Information("任务发送MES成功，lotid：{0}", lot.CoverTrayId);
                }
                else
                {
                    string errorstring = String.Format("向MES发送已完成任务失败,TrayGroupName:{0},失败原因：{1}", returnmessage.TRAYGROUPNAME, returnmessage.DESCRIPTION);
                    //MesLogClass.Logger.Error(errorstring);
                    throw new MesMessageException(errorstring);
                }
            }
        }
        public void LogMesMessage(string xmlstring)
        {
            XDocument doc = XDocument.Parse(xmlstring);
            Log.MesLog.Information("记录MES消息信息，Time ：{0} \t {1}", DateTime.Now, doc.ToString());
        }
        public void LogMesMessage(XmlDocument xmlstring)
        {
            XDocument doc = XDocument.Parse(xmlstring.InnerXml);
            Log.MesLog.Information("记录MES消息信息，Time ：{0} \t {1}", DateTime.Now, doc.ToString());
        }
    }
    public class RemoteTrayGroupInfoDownloadRequest
    {
        MesMessageHeader header;
        RemoteTrayGroupInfoDownloadRequestMessageBody Body;
        MesMessageReturn rt;
        public RemoteTrayGroupInfoDownloadRequest(string FGcode, string productType)
        {
            header = new MesMessageHeader(MesMessageType.RemoteTrayGroupInfoDownloadRequest);
            Body = new RemoteTrayGroupInfoDownloadRequestMessageBody(FGcode, productType);
            rt = new MesMessageReturn();
        }
        public XmlDocument GetXmlDocument()
        {
            XmlDocument newDoc = new XmlDocument();
            XmlElement root = newDoc.DocumentElement;
            newDoc.InsertBefore(newDoc.CreateXmlDeclaration("1.0", "utf-16", null), root);
            XmlElement newmessage = newDoc.CreateElement("Message");

            newmessage.AppendChild(header.GetElement(newDoc));
            newmessage.AppendChild(Body.GetElement(newDoc));
            newmessage.AppendChild(rt.GetElement(newDoc));

            newDoc.AppendChild(newmessage);
            return newDoc;
        }
        public Message GetMessage()
        {
            Message newmessage = new Message();
            newmessage.AddField("xmlData", GetXmlDocument().InnerXml);
            return newmessage;
        }
        public void SaveDoc(string path)
        {
            var doc = GetXmlDocument();
            doc.Save(path);
        }
    }
    public class RemoteTrayGroupInfoDownloadRequestMessageBody
    {
        public string PRODUCTSPECNAME; //FG CODE;
        public string PRODUCTIONTYPE;
        public string PROCESSOPERATIONNAME;
        public string MACHINENAME;

        public RemoteTrayGroupInfoDownloadRequestMessageBody(string pRODUCTSPECNAME, string pRODUCTIONTYPE)
        {
            PRODUCTSPECNAME = pRODUCTSPECNAME;
            PRODUCTIONTYPE = pRODUCTIONTYPE;
            PROCESSOPERATIONNAME = "C52000E";
            MACHINENAME = "7CTCT33";
        }
        public XmlElement GetElement(XmlDocument doc)
        {
            XmlElement newele = doc.CreateElement("Body");

            var newNodeList = GetXmlNodeList(doc);
            foreach (var item in newNodeList)
            {
                newele.AppendChild(item);
            }
            return newele;
        }
        public List<XmlNode> GetXmlNodeList(XmlDocument doc)
        {
            List<XmlNode> newlist = new List<XmlNode>();
            Type T = typeof(RemoteTrayGroupInfoDownloadRequestMessageBody);
            var filed = T.GetFields();
            foreach (var item in filed)
            {
                XmlNode newNode = doc.CreateNode(XmlNodeType.Element, item.Name, "");
                var a = item.GetValue(this);
                string newText = a.ToString();
                newNode.InnerText = newText;
                newlist.Add(newNode);
            }
            return newlist;
        }
    }
    public class RemoteTrayGroupProcessEnd
    {
        MesMessageHeader header;
        RemoteTrayGroupProcessEndMessageBody Body;
        MesMessageReturn rt;
        public RemoteTrayGroupProcessEnd(MesLot lot)
        {
            header = new MesMessageHeader(MesMessageType.RemoteTrayGroupProcessEnd);
            Body = new RemoteTrayGroupProcessEndMessageBody(lot);
            rt = new MesMessageReturn();
        }
        public XmlDocument GetXmlDocument()
        {
            XmlDocument newDoc = new XmlDocument();
            XmlElement root = newDoc.DocumentElement;
            newDoc.InsertBefore(newDoc.CreateXmlDeclaration("1.0", "utf-16", null), root);
            XmlElement newmessage = newDoc.CreateElement("Message");

            newmessage.AppendChild(header.GetElement(newDoc));
            newmessage.AppendChild(Body.GetElement(newDoc));
            newmessage.AppendChild(rt.GetElement(newDoc));

            newDoc.AppendChild(newmessage);
            return newDoc;
        }

        public Message GetMessage()
        {
            Message newmessage = new Message();
            newmessage.AddField("xmlData", GetXmlDocument().InnerXml);
            return newmessage;
        }
        public void Save(string path)
        {
            var document = GetXmlDocument();
            document.Save(path);
        }
    }
    public class RemoteTrayGroupProcessEndMessageBody
    {
        MesLot finishedlot;

        public RemoteTrayGroupProcessEndMessageBody(MesLot lot)
        {
            finishedlot = lot;
        }
        public XmlElement GetElement(XmlDocument doc)
        {
            XmlElement newele = doc.CreateElement("Body");

            var lotid = doc.CreateNode(XmlNodeType.Element, "TRAYGROUPNAME", "");
            lotid.InnerText = finishedlot.CoverTrayId;
            newele.AppendChild(lotid);

            var eqid = doc.CreateNode(XmlNodeType.Element, "MACHINENAME", "");
            eqid.InnerText = "7CTCT33";
            newele.AppendChild(eqid);

            var newPanelList = GetPanelList(doc);
            newele.AppendChild(newPanelList);

            return newele;
        }
        public XmlElement GetPanelList(XmlDocument doc)
        {
            XmlElement newele = doc.CreateElement("PANELLIST");
            foreach (var item in finishedlot.missions)
            {
                var newpanel = doc.CreateNode(XmlNodeType.Element, "PANEL", "");
                var PANELNAME = doc.CreateNode(XmlNodeType.Element, "PANELNAME", "");
                var LOTGRADE = doc.CreateNode(XmlNodeType.Element, "LOTGRADE", "");
                var LOTDETAILGRADE = doc.CreateNode(XmlNodeType.Element, "LOTDETAILGRADE", "");
                var USERID = doc.CreateNode(XmlNodeType.Element, "USERID", "");

                PANELNAME.InnerText = item.PanelId;
                newpanel.AppendChild(PANELNAME);
                LOTGRADE.InnerText = item.LotGrade.ToString();
                newpanel.AppendChild(LOTGRADE);
                LOTDETAILGRADE.InnerText = item.PanelJudge.ToString();
                newpanel.AppendChild(LOTDETAILGRADE);
                USERID.InnerText = "10086";
                newpanel.AppendChild(USERID);

                // 添加panel defect；
                var DEFECTLIST = doc.CreateNode(XmlNodeType.Element, "DEFECTLIST", "");
                if (item.PanelJudge != JudgeGrade.W && item.PanelJudge != JudgeGrade.S && item.PanelJudge != JudgeGrade.A)
                {
                    var DEFECT = doc.CreateNode(XmlNodeType.Element, "DEFECT", "");
                    var DEFECTCODE = doc.CreateNode(XmlNodeType.Element, "DEFECTCODE", "");
                    DEFECTCODE.InnerText = item.Judge.FinalJudge.ToString();
                    DEFECT.AppendChild(DEFECTCODE);
                    DEFECTLIST.AppendChild(DEFECT);
                }
                newpanel.AppendChild(DEFECTLIST);
                // 添加该 panel；
                newele.AppendChild(newpanel);
            }
            return newele;
        }
    }
    public class RemoteTrayGroupProcessEndReply
    {
        XmlDocument OriginalDoc;
        public string TRAYGROUPNAME;
        public string RESULT;
        public string DESCRIPTION;
        public bool Result
        {
            get
            {
                if (RESULT == "OK")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public RemoteTrayGroupProcessEndReply(XmlDocument doc)
        {
            OriginalDoc = doc;
            TRAYGROUPNAME = InitialField("TRAYGROUPNAME");
            RESULT = InitialField("RESULT");
            DESCRIPTION = InitialField("DESCRIPTION");
        }
        string InitialField(string tagName)
        {
            var tRAYGROUPNAME = OriginalDoc.GetElementsByTagName(tagName)[0];
            if (tRAYGROUPNAME != null)
            {
                return tRAYGROUPNAME.InnerText;
            }
            else
            {
                string errorString = String.Format("{0}为空，请检查MES消息的完整性", tagName);
                throw new MesMessageException(errorString);
            }
        }
    }

    [Serializable]
    internal class MesMessageException : Exception
    {
        public MesMessageException()
        {
        }

        public MesMessageException(string message) : base(message)
        {
        }

        public MesMessageException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MesMessageException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class MesMessageHeader
    {
        public MesMessageType MESSAGENAME;  // 声明消息的用途（请求任务或完成任务）PanelProcessEnd等；
        public string TRANSACTIONID;  //要求唯一，作为MES分辨消息的查询依据（可能是数据库主键）2021072017235803711；
        public string ORIGINALSOURCESUBJECTNAME = "";
        public string SOURCESUBJECTNAME = "BOE.B7.MEM.PRD.7CTCT33";
        public string TARGETSUBJECTNAME = "BOE.B7.MEM.PRD.PEMsvr";
        public string SHOPNAME = "EAC2";
        public string MACHINENAME = "7CTCT33";

        public MesMessageHeader(MesMessageType mESSAGENAME)
        {
            MESSAGENAME = mESSAGENAME;
            // 每次发送时应更新
            TRANSACTIONID = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
        }
        public XmlElement GetElement(XmlDocument doc)
        {
            XmlElement newele = doc.CreateElement("Header");

            var newNodeList = GetXmlNodeList(doc);
            foreach (var item in newNodeList)
            {
                newele.AppendChild(item);
            }
            return newele;
        }
        public List<XmlNode> GetXmlNodeList(XmlDocument doc)
        {
            List<XmlNode> newlist = new List<XmlNode>();
            Type T = typeof(MesMessageHeader);
            var filed = T.GetFields();
            foreach (var item in filed)
            {
                XmlNode newNode = doc.CreateNode(XmlNodeType.Element, item.Name, "");
                string newText = item.GetValue(this).ToString();
                newNode.InnerText = newText;
                newlist.Add(newNode);
            }
            return newlist;
        }
    }
    public class MesMessageReturn
    {
        string RETURNCODE;
        string RETURNMESSAGE;

        public MesMessageReturn()
        {
            RETURNCODE = "0";
            RETURNMESSAGE = "";
        }
        public XmlElement GetElement(XmlDocument doc)
        {
            XmlElement newele = doc.CreateElement("Return");
            XmlElement rcode = doc.CreateElement("RETURNCODE");
            rcode.InnerText = RETURNCODE;
            XmlElement rmessage = doc.CreateElement("RETURNMESSAGE");
            rmessage.InnerText = RETURNMESSAGE;
            newele.AppendChild(rcode);
            newele.AppendChild(rmessage);
            return newele;
        }
    }
    public enum MesMessageType
    {
        RemoteTrayGroupInfoDownloadRequest,     //向MES请求任务并hold lot；
        OpCallSend,                             //如果发送的请求失败（如E站点没有待检LOT），返回的报错信息；
        RemoteTrayGroupInfoDownloadSend,        //从MES发回的任务，包含整个lot 的信息；
        RemoteTrayGroupProcessEnd,              //LOT检查完成，向MES发送检查结果；
        RemoteTrayGroupProcessEndReply,         //MES回复检查收到检查结果是否处理成功；
    }
}
