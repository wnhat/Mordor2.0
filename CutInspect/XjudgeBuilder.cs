using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CutInspect
{
    public class XjudgeBuilder
    {
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

            return xmlDoc.InnerXml;
        }
    }
}
