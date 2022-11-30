using CoreClass;
using CoreClass.DICSEnum;
using CoreClass.Model;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newspider
{
    /// <summary>
    /// 从redis数据库中获取数据的速度 0.6s/次，主要是由于多次与数据库建立连接时造成的时间消耗（同时发起多次不会对性能有显著影响）
    /// 因此建议每次获取对应panel id 路径时加入所有需获取的panelid，id数量增多不会影响相应时间；
    /// 获取panel path的方法在 redisconnecter中；
    /// </summary>
    public static class FilePathManager
    {
        static List<PC> InsPCList = IpTransform.Name2IP(new Pcinfo[] { Pcinfo.AVI, Pcinfo.SVI });
        static List<HardDisk> DiskCollection = new List<HardDisk>();

        //public static event EventHandler<DiskStatusChangedEventArgs> DiskStatusChangedEvent;

        static FilePathManager()
        { 
            // Initialize
            // get disk searchinfo from redis server to initial class;
            foreach (var disk in (Disk[])Enum.GetValues(typeof(Disk)))
            {
                if (disk != Disk.Null)
                {
                    foreach (var pc in InsPCList)
                    {
                        // check if diskinfo exist;
                        string diskInfoQueryString = "path:info:" + pc.PcIp + ":" + disk.ToString();
                        var info = RedisConnector.Redis.StringGet(diskInfoQueryString);
                        // if database has no info,initial a new disk class;
                        HardDisk hardDisk;
                        if (info.HasValue)
                        {
                            hardDisk = BsonSerializer.Deserialize<HardDisk>(info.ToString());
                        }
                        else
                        {
                            hardDisk = new HardDisk(pc,disk);
                            UpdateDiskInfo(hardDisk);
                        }
                        DiskCollection.Add(hardDisk);
                    };
                }
            };
            Loger.Testlogger.Information("PanelPathManager DiskInfo initilize finished;");
        }
        /// <summary>
        /// 慎用！！！
        /// </summary>
        public static void ClearRedisInfo()
        {
            RedisConnector.Redis.KeyDelete("path:info:list");
            foreach (var disk in (Disk[])Enum.GetValues(typeof(Disk)))
            {
                if (disk != Disk.Null)
                {
                    foreach (var pc in InsPCList)
                    {
                        string diskInfoQueryString = "path:info:" + pc.PcIp + ":" + disk.ToString();
                        var info = RedisConnector.Redis.KeyDelete(diskInfoQueryString);
                    };
                }
            };
        }
        static void UpdateDiskInfo(HardDisk disk)
        {
            BsonDocument buffer = new BsonDocument();
            var writer = new BsonDocumentWriter(buffer);
            BsonSerializer.Serialize<HardDisk>(writer, disk);
            var json = buffer.ToJson();
            RedisConnector.Redis.StringSet(disk.RedisInfoKey, json);
        }
        static void UpdateDiskPathCollection(HardDisk disk,IEnumerable<string> path)
        {
            var length = RedisConnector.Redis.SetLength(disk.RedisPathKey);
            if (length != 0)
            {
                DeleteDiskPathCollectionOnRedis(disk);
            }
            List<RedisValue> redisValues = new List<RedisValue>();
            foreach (var item in path)
            {
                redisValues.Add(item);
            }
            RedisConnector.Redis.SetAddAsync(HardDisk.RedisInfoListKey, disk.RedisPathKey);
            RedisConnector.Redis.SetAddAsync(disk.RedisPathKey, redisValues.ToArray());
        }
        static void DeleteDiskPathCollectionOnRedis(HardDisk disk)
        {
            // TODO: 修改存在的key储存列表；
            RedisConnector.Redis.SetRemove(HardDisk.RedisInfoListKey, disk.RedisPathKey);
            RedisConnector.Redis.KeyDelete(disk.RedisPathKey);
        }
        public static void RefreshFileList()
        {
            Loger.Logger.Information("start to refresh the file dict, time ： {0}", DateTime.Now);

            Task looptask = Task.Run(() => { Parallel.For(0, DiskCollection.Count - 1, i => { Refreshdisk(DiskCollection[i]); }); });
            if (looptask.Wait(150000))
            {
                Loger.Logger.Information("路径搜寻正常完成；");
            }
            else
            {
                Loger.Logger.Error("路径搜寻超过设定时间已被取消，请调查问题原因；");
            }
            Loger.Logger.Information("finished Refresh, time is {0}", DateTime.Now);
            GC.Collect();
        }
        public static void Refreshdisk(HardDisk disk)
        {
            DiskStatus pastStatus = disk.Status;
            try
            {
                var newTime = Directory.GetLastWriteTimeUtc(disk.OriginPath);
                // 查看目录文件夹是否更新；
                if (disk.LastSearchTime != newTime)
                {
                    HashSet<string> newPath = GetDiskPathCollection(disk);
                    UpdateDiskPathCollection(disk, newPath);
                    UpdateDiskInfo(disk);
                }
                else
                {
                    // 文件夹未变动，pass；
                }
            }
            catch (UnauthorizedAccessException e)
            {
                // 硬盘损坏，该硬盘路径无法通过远程访问的方式打开
                //FilePathLogClass.Logger.Debug(e.Message);
                disk.Status = DiskStatus.ConnectError;
                //disk.lastErrorMessage = e.Message;
            }
            catch (DirectoryNotFoundException e)
            {
                // 硬盘损坏，或该路径下硬盘无实物存在，该硬盘路径无法通过远程访问的方式打开
                //FilePathLogClass.Logger.Debug(e.Message);
                disk.Status = DiskStatus.NotExist;
                //disk.lastErrorMessage = e.Message;
            }
            catch (IOException e)
            {
                // 网络通信问题，网络无法链接到该计算机，可能是因为该计算连接交换机的网线出现了故障，或网卡断开，需要检查设备的硬件原因；
                //FilePathLogClass.Logger.Debug(e.Message);
                disk.Status = DiskStatus.ConnectError;
                //disk.lastErrorMessage = e.Message;
            }
            catch (Exception e)
            {
                //FilePathLogClass.Logger.Error(e.Message);
                disk.Status = DiskStatus.ConnectError;
                //disk.lastErrorMessage = e.Message;
            }
            finally
            {
                if (disk.Status != DiskStatus.OK)
                {
                    DeleteDiskPathCollectionOnRedis(disk);
                }
                if (disk.Status != pastStatus)
                {
                    string statusChangedInfo;
                    if (disk.Status == DiskStatus.OK)
                    {
                        statusChangedInfo = "新添加硬盘；";
                    }
                    else
                    {
                        statusChangedInfo = "硬盘损坏；";
                    }
                    //DiskStatusChangedEventArgs newevent = new DiskStatusChangedEventArgs(disk, pastStatus, statusChangedInfo);
                    //DiskStatusChangedEvent?.Invoke(null, newevent);
                }
            }
        }
        public static HashSet<string> GetDiskPathCollection(HardDisk disk)
        {
            disk.Status = DiskStatus.OnSearch;
            UpdateDiskInfo(disk);

            string[] image_directory_list = Directory.GetDirectories(disk.OriginPath);
            string[] result_directory_list = Directory.GetDirectories(disk.ResultPath);

            disk.Status = DiskStatus.OK;
            var intersectlist = Enumerable.Intersect(image_directory_list, result_directory_list, new StringPathCompare());
            HashSet<string> panelIdList = new HashSet<string>();
            foreach (var item in intersectlist)
            {
                panelIdList.Add(Path.GetFileName(item));
            }
            return panelIdList;
        }
        internal class StringPathCompare : IEqualityComparer<string>
        {//用于图像及result文件夹路径是否位于同一硬盘的对比；
            public bool Equals(string x, string y)
            {
                if (Path.GetFileName(x) == Path.GetFileName(y))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            public int GetHashCode(string obj)
            {
                return Path.GetFileName(obj).GetHashCode();
            }
        }
    }
}
