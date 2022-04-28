using CoreClass;
using CoreClass.Model;
using NetMQ;
using NetMQ.Sockets;

namespace EyeOfSauron
{
    public static class SeverConnector
    {
        private static readonly RequestSocket Request;
        
        static SeverConnector()
        {
            Request = new RequestSocket();
            Request.Connect("tcp://localhost:5555");
            //Request.Connect("tcp://172.16.150.100:5555");
        }

        /// <summary>
        ///Will not get ReceiveSignal for now;
        /// </summary>
        /// <param name="operatorJudge"></param>
        /// <param name="mission"></param>
        public static void SendPanelMissionResult(OperatorJudge operatorJudge, InspectMission mission)
        {
            OperatorJudgeMessage ResultMessage = new(operatorJudge, mission);
            //Request.SendMultipartMessage(ResultMessage);
            //_ = Request.ReceiveSignal();
        }
    }
}
