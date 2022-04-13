using System;
using CoreClass.LogSpider;
using CoreClass.Model;
using CoreClass;
using CoreClass.DICSEnum;
using System.Collections.Generic;
using CoreClass.Element;
using MongoDB.Driver;
using MongoDB.Bson;
using System.IO;
using NetMQ;
using NetMQ.Sockets;
using System.Threading.Tasks;
using System.Threading;

namespace Spider
{
    class Program
    {
        static void Main(string[] args)
        {
            // get parameter from args;
            SpiderParameter.initialize(args);
            Run();
        }
        static void Run()
        {
            Spider.Run();
        }
    }
    public static class SpiderParameter
    {
        public static string Pcip;
        internal static void initialize(string[] args)
        {
            if (args == null)
            {
                args = new string[] { "172.16.200.100" };
            }
            // "@tcp://172.16.210.22:5554";
            Pcip = @"@tcp://" + args[0] + ":5554";
        }
    }
}
