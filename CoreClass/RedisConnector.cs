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
        static public List<PanelPathContainer> GetPanelPath(string panelId)
        {
            List<PanelPathContainer> pathlist = new List<PanelPathContainer>();
            // （已完成 添加 pathcontainer lock）当pathcontainer更新时，未完成的更新操作会阻断正在进行的enumration操作，导致部分ID查找不到；
            var disklist = RedisConnector.Redis.SetMembers(HardDisk.RedisInfoListKey);
            foreach (var item in disklist)
            {
                string diskInfoKey = item.ToString();
                if (RedisConnector.Redis.SetContains(diskInfoKey, panelId))
                {
                    string[] pcip = diskInfoKey.Split(':');
                    PC pC = IpTransform.GetPC(pcip[2]);
                    Disk disk = Enum.Parse<Disk>(pcip[3]);
                    pathlist.Add(new PanelPathContainer(panelId, pC, disk));
                }
            }
            if (pathlist.Count == 0)
            {
                return null;
            }
            return pathlist;
        }
        static public Dictionary<string, List<PanelPathContainer>> GetPanelPathList(string[] panelIdList)
        {
            // 重复的ID会导致返回相同的路径；因此将ID进行删除重复项的操作；
            panelIdList = panelIdList.Distinct().ToArray();
            Dictionary<string, List<PanelPathContainer>> container = new Dictionary<string, List<PanelPathContainer>>();

            var disklist = RedisConnector.Redis.SetMembers(HardDisk.RedisInfoListKey);
            foreach (var item in disklist)
            {
                string diskInfoKey = item.ToString();
                // 将panelid 初始化为 redis value；
                RedisValue[] redisValues = panelIdList.ToRedisValueArray();
                for (int i = 0; i < panelIdList.Length; i++)
                {
                    redisValues[i] = panelIdList[i];
                }
                var result = RedisConnector.Redis.SetContains(diskInfoKey, redisValues);
                // 将存在的panel path 插入容器当中；
                for (int i = 0; i < panelIdList.Length; i++)
                {
                    bool exist = result[i];
                    if (exist)
                    {
                        string[] pcip = diskInfoKey.Split(':');
                        PC pC = IpTransform.GetPC(pcip[2]);
                        Disk disk = Enum.Parse<Disk>(pcip[3]);
                        string panelid = panelIdList[i];
                        PanelPathContainer newpath = new PanelPathContainer(panelid, pC, disk);

                        if (container.ContainsKey(panelid))
                        {
                            container[panelid].Add(newpath);
                        }
                        else
                        {
                            container.Add(panelid, new List<PanelPathContainer> { newpath });
                        }
                    }
                }
            }
            foreach (var item in panelIdList)
            {
                if (!container.ContainsKey(item))
                {
                    container.Add(item, null);
                }
            }
            return container;
        }
    }
}
