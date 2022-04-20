using CoreClass;
using CoreClass.Model;
using NetMQ;
using NetMQ.Sockets;

namespace EyeOfSauron
{
    public static class SeverConnector
    {
        //Old method to connect to the server
        private static readonly RequestSocket Request;
        static SeverConnector()
        {
            Request = new RequestSocket();
            Request.Connect("tcp://localhost:5555");
            //Request.Connect("tcp://172.16.150.100:5555");
        }
        public static void SendPanelMissionResult(OperatorJudge operatorJudge, InspectMission mission)
        {
            OperatorJudgeMessage ResultMessage = new(operatorJudge, mission);
            Request.SendMultipartMessage(ResultMessage);
            _ = Request.ReceiveSignal();
        }
    }
}
