using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreClass.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
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
        address,
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
            // convert version from bsondocument;
            Version = BsonSerializer.Deserialize<VersionCheckClass>(message[(int)MessageFieldName.Version].Buffer);
        }
        public BaseMessage(MessageType theMessageType)
        {
            TheMessageType = theMessageType;
            Version = StaticVersion.Version;
            this.Append((int)TheMessageType);
            this.Append(Version.ToBson());
        }
    }
    public class PanelPathMessage : BaseMessage
    {
        public Dictionary<string, List<PanelPathContainer>> panelPathDic;
        //序列化发送的Massage
        public PanelPathMessage(Dictionary<string, List<PanelPathContainer>> panelpathdic) : base(MessageType.CLINET_GET_PANEL_PATH)
        {
            panelPathDic = panelpathdic;
            this.Append(panelPathDic.ToBson());
        }
        //对收到的Massage进行反序列化
        public PanelPathMessage(NetMQMessage message) : base(message)
        {
            panelPathDic = BsonSerializer.Deserialize<Dictionary<string, List<PanelPathContainer>>>(message[(int)MessageFieldName.Field1].Buffer);
        }
    }
    public class OperatorJudgeMessage : BaseMessage
    {
        public OperatorJudge Judge;
        public InspectMission Mission;
        public OperatorJudgeMessage(OperatorJudge operatorJudge, InspectMission mission) : base(MessageType.CLIENT_SEND_MISSION_RESULT)
        {
            Judge = operatorJudge;
            this.Append(Judge.ToBson());
            Mission = mission;
            this.Append(Mission.ToBson());
        }
        public OperatorJudgeMessage(NetMQMessage theMessage) : base(theMessage)
        {
            Judge = BsonSerializer.Deserialize<OperatorJudge>(theMessage[(int)MessageFieldName.Field1].Buffer);
            Mission = BsonSerializer.Deserialize<InspectMission>(theMessage[(int)MessageFieldName.Field2].Buffer);
        }
    }
}