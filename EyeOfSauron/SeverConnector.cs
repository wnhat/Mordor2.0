using CoreClass;
using CoreClass.Model;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Threading.Tasks;

namespace EyeOfSauron
{
    public static class SeverConnector
    {
        private static readonly DealerSocket Request = new();

        static SeverConnector()
        {
            Request.Connect("tcp://172.16.210.22:5555");
        }

        /// <summary>
        ///Will not get ReceiveSignal for now;
        /// </summary>
        /// <param name="operatorJudge"></param>
        /// <param name="mission"></param>
        public static bool SendPanelMissionResult(OperatorJudge operatorJudge, InspectMission mission)
        {
            OperatorJudgeMessage ResultMessage = new(operatorJudge, mission);
            Request.SendMultipartMessage(ResultMessage);
            NetMQMessage? netMQFrames = new();
            return Request.TryReceiveMultipartMessage(TimeSpan.FromMilliseconds(100),ref netMQFrames);
        }
    }
}
