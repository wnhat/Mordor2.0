using CoreClass;
using NetMQ;
using NetMQ.Sockets;
using System;

namespace Sauron
{
    class Program
    {
        static void Main(string[] args)
        {
            //Sauron.Run(); //启动服务器；
            Test();
        }
        static void Test()
        {
            MissionManager testmanager = new MissionManager();
            testmanager.AddMissionTest();
        }
    }
}
