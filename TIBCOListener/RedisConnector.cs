using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIBListener
{
    public static class RedisConnector
    {
        static readonly string[] ConnectString = {
            "172.16.220.35:6379",
            "172.16.220.55:6379",
            "172.16.220.65:6379"
        };
        static readonly string TestHost = "localhost:6379";
        static string RedisServer
        {
            get
            {
                string buf = "";
                foreach (string str in ConnectString)
                {
                    buf += (str + ",");
                }
                return buf.Substring(0, buf.Length - 1);
            }
        }
        static readonly ConnectionMultiplexer Connecter = ConnectionMultiplexer.Connect(RedisServer);
        public static IDatabase Redis { get { return Connecter.GetDatabase(); } }
    }
}
