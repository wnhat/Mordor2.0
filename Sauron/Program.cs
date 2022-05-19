using CoreClass;
using CoreClass.Model;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sauron
{
    class Program
    {
        static void Main(string[] args)
        {
            DBconnector.InitialDB();
            //Sauron.Run(); //启动服务器；
            Test();
        }
        static void Test()
        {
            MissionManager testmanager = new MissionManager();
            Task.Run(Sauron.Run);
            Task.Run(TestClient);
            while (true)
            {
                Log.Testlogger.Information("添加测试任务");
                testmanager.AddMissionTest();
                Thread.Sleep(300000);
            }
        }
        static void TestClient()
        {
            Thread.Sleep(15000);
            DealerSocket Request;
            Request = new DealerSocket();
            Request.Connect("tcp://172.16.210.22:5555");
            User op = User.AutoJudgeUser;
            Random defectrandom = new Random();
            while (true)
            {
                // 随机获取defect;
                Defect defect = Defect.AllDefects[defectrandom.Next(0, Defect.AllDefects.Count)];
                OperatorJudge judge = new OperatorJudge(defect, op.Username, op.Account, op.Id,defectrandom.Next(3500,4500));
                // 获取检查任务;
                InspectMission mission = InspectMission.GetMission();
                if (mission == null)
                {
                    break;
                }

                OperatorJudgeMessage ResultMessage = new(judge, mission);

                Log.Testlogger.Information("测试：向server 发送judge");
                Request.SendMultipartMessage(ResultMessage);
                Thread.Sleep(10);
            }
            Log.Testlogger.Information("无剩余任务测试结束，线程退出");
        }
    }
}
