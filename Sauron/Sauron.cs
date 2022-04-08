using CoreClass;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sauron
{
    public static class Sauron
    {
        // TODO: Use DB parameter to control refresh span;
        static NetMQTimer MissionTimer = new NetMQTimer(TimeSpan.FromSeconds(3600));
        static RouterSocket routerSocket = new RouterSocket("@tcp://172.16.200.100:5555");
        static NetMQPoller poller = new NetMQPoller { routerSocket, MissionTimer };
        static MissionManager manager = new MissionManager();

        static Sauron()
        {
            // 为poller绑定触发事件；
            routerSocket.ReceiveReady += OnMessageArrive;
            MissionTimer.Elapsed += MissionAdd;
            // add a new timer to poller and refresh defect list;
            
        }
        public static void Run()
        {
            poller.Run();
        }
        static void MissionAdd(object sender, NetMQTimerEventArgs eventArgs)
        {
            Task.Run(manager.AddAutoMission);
        }
        static void OnMessageArrive(object sender, NetMQSocketEventArgs eventArgs)
        {
            /* 对客户端发送的事件进行分Type响应（按照Message首位）*/
            NetMQMessage messageIn = eventArgs.Socket.ReceiveMultipartMessage();
            // 转换为自定义Message类型;
            BaseMessage switchmessage = new BaseMessage(messageIn);
            if (switchmessage.TheMessageType == MessageType.CLIENT_SEND_MISSION_RESULT)
            {
                OperatorJudgeMessage message = new OperatorJudgeMessage(messageIn);
                // 完成检查任务；
                manager.FinishMission(message.Judge,message.Mission);
            }
        }
        // refesh defect list every 5 minutes;
        public static void RefreshDefectList()
        {
            try
            {
                Task.Run(Defects.RefreshDefectList);
            }
            catch (Exception e)
            {
                // log exception details;
                Log.Logger.Error(e.Message);
                throw;
            }
        }
    }
}
