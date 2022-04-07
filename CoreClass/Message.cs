using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreClass.Model;
using NetMQ;
using Newtonsoft.Json;

namespace CoreClass
{
    public enum MessageType
    {
        VERSION_CHECK,
        VERSION_ERROR,

        CLIENT_SEND_MISSION_RESULT = 10,
        CLIENT_SEND_EXAM_RESULT,
        CLINET_GET_PANEL_MISSION,
        CLINET_GET_PANEL_PATH,
        CLINET_GET_EXAM_MISSION_LIST,
        CLINET_GET_EXAMINFO,
        CLINET_CHECK_USER,
        CLINET_GET_PRODUCTINFO,

        CONTROLER_CLEAR_MISSION = 100,
        CONTROLER_ADD_MISSION,
        CONTROLER_REFRESH_EXAM,

        SERVER_SEND_MISSION = 200,
        SERVER_SEND_FINISH,
        SERVER_SEND_EORRO,
        SERVER_SEND_EXAMINFO,
        SERVER_SEND_PANEL_GREAD,
        SERVER_SEND_USER_FLASE,
        SERVER_SEND_USER_TRUE,
        SERVER_SEND_PRODUCTINFO,
    }
    public enum MessageFieldName
    {
        MessageType,
        Version,
        Field1,
        Field2,
        Field3,
    }
    public class BaseMessage : NetMQMessage
    {
        public MessageType TheMessageType;
        public VersionCheckClass Version;

        public BaseMessage(NetMQMessage message)
        {
            TheMessageType = (MessageType)message[((int)MessageFieldName.MessageType)].ConvertToInt32();
            Version = TransferToVersion(message[(int)MessageFieldName.Version].ConvertToString());
        }
        public BaseMessage(MessageType theMessageType)
        {
            TheMessageType = theMessageType;
            Version = StaticVersion.Version;
            this.Append((int)TheMessageType);
            this.Append(TransferToString(Version));
        }
        public BaseMessage(MessageType theMessageType, VersionCheckClass version)
        {
            TheMessageType = theMessageType;
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }
            Version = version;
            this.Append((int)TheMessageType);
            this.Append(TransferToString(version));

            if (!Version.CheckVersion(StaticVersion.Version))
            {
                throw new VersionException("消息的版本检查异常，请确认客户端的版本；");
            }
        }
        string TransferToString(VersionCheckClass version)
        {
            return JsonConvert.SerializeObject(version, JsonSerializerSetting.Setting);
        }
        VersionCheckClass TransferToVersion(string versionstring)
        {
            return JsonConvert.DeserializeObject<VersionCheckClass>(versionstring, JsonSerializerSetting.Setting);
        }
    }
    public class PanelPathMessage : BaseMessage
    {
        public Dictionary<string, List<PanelPathContainer>> panelPathDic;
        //序列化发送的Massage
        public PanelPathMessage(Dictionary<string, List<PanelPathContainer>> panelpathdic) : base(MessageType.CLINET_GET_PANEL_PATH, StaticVersion.Version)
        {
            panelPathDic = panelpathdic;
            this.Append(TransferToString(panelPathDic));
        }
        public PanelPathMessage(string[] panelidarray) : base(MessageType.CLINET_GET_PANEL_PATH, StaticVersion.Version)
        {
            panelPathDic = new Dictionary<string, List<PanelPathContainer>>();
            foreach (var item in panelidarray)
            {
                // 为了客户端请求任务时不多写一个传送ID List 的消息，用该字典装载请求的id信息；
                panelPathDic.Add(item, null);
            }
            this.Append(TransferToString(panelPathDic));
        }
        //对收到的Massage进行反序列化
        public PanelPathMessage(NetMQMessage message) : base(message)
        {
            panelPathDic = TransferToResult(message[(int)MessageFieldName.Field1].ConvertToString());
        }
        //序列化和反序列化实现
        string TransferToString(Dictionary<string, List<PanelPathContainer>> result)
        {
            return JsonConvert.SerializeObject(result, JsonSerializerSetting.Setting);
        }
        Dictionary<string, List<PanelPathContainer>> TransferToResult(string resultstring)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, List<PanelPathContainer>>>(resultstring, JsonSerializerSetting.Setting);
        }
    }
    public class ProductInfoMessage : BaseMessage
    {
        public List<ProductInfo> InfoList;
        public ProductInfoMessage(List<ProductInfo> list) : base(MessageType.SERVER_SEND_PRODUCTINFO, StaticVersion.Version)
        {
            InfoList = list;
            this.Append(TransferToString(InfoList));
        }
        public ProductInfoMessage(NetMQMessage theMessage) : base(theMessage)
        {
            InfoList = TransferToMission(theMessage[(int)MessageFieldName.Field1].ConvertToString());
        }
        string TransferToString(List<ProductInfo> panelMission)
        {
            return JsonConvert.SerializeObject(panelMission, JsonSerializerSetting.Setting);
        }
        List<ProductInfo> TransferToMission(string missionstring)
        {
            return JsonConvert.DeserializeObject<List<ProductInfo>>(missionstring, JsonSerializerSetting.Setting);
        }
    }
    public class OperatorJudgeMessage : BaseMessage
    {
        public OperatorJudge judge;
        public OperatorJudgeMessage(OperatorJudge operatorJudge) : base(MessageType.CLIENT_SEND_MISSION_RESULT, StaticVersion.Version)
        {
            judge = operatorJudge;
            this.Append(TransferToString(judge));
        }
        public OperatorJudgeMessage(NetMQMessage theMessage) : base(theMessage)
        {
            judge = TransferTojudge(theMessage[(int)MessageFieldName.Field1].ConvertToString());
        }
        string TransferToString(OperatorJudge operatorJudge)
        {
            return JsonConvert.SerializeObject(operatorJudge, JsonSerializerSetting.Setting);
        }
        OperatorJudge TransferTojudge(string operatorJudge)
        {
            return JsonConvert.DeserializeObject<OperatorJudge>(operatorJudge, JsonSerializerSetting.Setting);
        }
    }
}