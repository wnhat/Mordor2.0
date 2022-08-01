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
            //DBconnector.InitialDB();
            //MissionManager missionManager = new MissionManager();
            //missionManager.AddMissionTest();
            //Console.WriteLine("启动完成");
            Sauron.Run(); //启动服务器；
        }
    }
}
