using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreClass.DICSEnum;
using CoreClass.Model;
using StackExchange.Redis;

namespace CoreClass
{
    public static class RedisConnector
    {
        static readonly string[] ConnectString = {
            "172.16.220.35:6379",
            "172.16.220.55:6379",
            "172.16.220.95:6379",
            "172.16.220.75:6379",
            "172.16.220.85:6379",
            "172.16.220.65:6379"
        };
        static readonly string TestHost = "localhost:6379";
        static string RedisServer {
            get {
                string buf = "";
                foreach (string str in ConnectString)
                {
                    buf += (str + ",");
                }
                return buf.Substring(0, buf.Length - 1);
            } }
        static readonly ConnectionMultiplexer Connecter = ConnectionMultiplexer.Connect(RedisServer);
        public static IDatabase Redis { get { return Connecter.GetDatabase(); } }
        public static IServer[] Server { get { return Connecter.GetServers(); } }
        public static string Get(string key)
        {
            return Redis.StringGet(key);
        }
        public static HashEntry[] DiskValueGet(string key)
        {
            return Redis.HashGetAll(key);
        }
        public static void DeleteALL()
        {
            Redis.Execute("flushall");
        }
    }
}
