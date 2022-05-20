using CoreClass;
using CoreClass.Model;
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
        static NetMQTimer MissionTimer = new NetMQTimer(TimeSpan.FromMinutes(60));
        static NetMQTimer DefectTimer = new NetMQTimer(TimeSpan.FromMinutes(5));
        static RouterSocket routerSocket = new RouterSocket("@tcp://172.16.210.22:5555");
        static NetMQPoller poller = new NetMQPoller { routerSocket, MissionTimer, DefectTimer };
        static MissionManager manager = new MissionManager();
        static Sauron()
        {
            // 为poller绑定触发事件；
            routerSocket.ReceiveReady += OnMessageArrive;
            //MissionTimer.Elapsed += MissionAdd;
            DefectTimer.Elapsed += RefreshDefectList;
            // add a new timer to poller and refresh defect list;
        }
        public static void Run()
        {
            Log.Testlogger.Information("测试：sql server服务器启动；");
            Task.Run(OldDBconnector.MainCycle);
            Log.Testlogger.Information("测试：服务器启动");
            poller.Run();
        }
        static void MissionAdd(object sender, NetMQTimerEventArgs eventArgs)
        {
            Task.Run(MesMissionStorage.AddAutoMission);
        }
        static void OnMessageArrive(object sender, NetMQSocketEventArgs eventArgs)
        {
            /* 对客户端发送的事件进行分Type响应（按照Message首位）*/
            NetMQMessage messageIn = eventArgs.Socket.ReceiveMultipartMessage();
            
            // 向客户端发送消息确认连接；
            NetMQMessage ReturnMessage = new NetMQMessage();
            ReturnMessage.Append(messageIn.First);
            ReturnMessage.Append(0);
            eventArgs.Socket.SendMultipartMessage(ReturnMessage);
            
            Log.Testlogger.Information("测试：收到judge");
            // 转换为自定义Message类型;
            BaseMessage switchmessage = new BaseMessage(messageIn);
            if (switchmessage.TheMessageType == MessageType.CLIENT_SEND_MISSION_RESULT)
            {
                OperatorJudgeMessage message = new OperatorJudgeMessage(messageIn);
                // 完成检查任务；
                manager.FinishMission(message.Judge, message.Mission);
            }
            Log.Testlogger.Information("测试：judge完成");
        }
        // refesh defect list every 5 minutes;
        public static void RefreshDefectList(object sender, NetMQTimerEventArgs eventArgs)
        {
            Task.Run(
                () =>
                {
                    try
                    {
                        Defect.RefreshDefectList();
                    }
                    catch (Exception e)
                    {
                        // log exception details;
                        Log.Logger.Error(e,"在刷新Defect列表时发生异常");
                    }
                }
            );
        }
    }
}
