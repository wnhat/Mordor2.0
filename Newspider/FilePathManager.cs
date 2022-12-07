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
using System.Threading;
using System.Threading.Tasks;

namespace Newspider
{
    /// <summary>
    /// 从redis数据库中获取数据的速度 0.6s/次，主要是由于多次与数据库建立连接时造成的时间消耗（同时发起多次不会对性能有显著影响）
    /// 因此建议每次获取对应panel id 路径时加入所有需获取的panelid，id数量增多不会影响相应时间；
    /// 获取panel path的方法在 redisconnecter中；
    /// redis 数据库的解决方案在速度上并不能令人满意，因此在新版本中被抛弃了；
    /// 
    /// 现有解决方案继续保留 dict + hashset的解决方案，其他客户端通过coreclass中的连接组件，通过zeromq进行数据的访问；
    /// </summary>
    public static class FilePathManager
    {
        static List<PC> InsPCList = IpTransform.Name2IP(new Pcinfo[] { Pcinfo.AVI, Pcinfo.SVI });
        static Dictionary<HardDisk, HashSet<string>> DiskCollection = new Dictionary<HardDisk, HashSet<string>>();

        // public static event EventHandler<DiskStatusChangedEventArgs> DiskStatusChangedEvent;

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
                            hardDisk = new HardDisk(pc, disk);
                            UpdateDiskInfo(hardDisk);
                        }
                        DiskCollection.Add(hardDisk,null);
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
        /// <summary>
        /// 刷新设备中现存的结果文件路径，并更新内存中的记录；
        /// TODO：对每次发生状态变动（损坏、新增、等）的硬盘进行记录，每次生成记录存储在mongoDB中；
        /// </summary>
        public static void RefreshFileList()
        {
            Loger.Logger.Information("start to refresh the file dict, time ： {0}", DateTime.Now);

            // 当任务取消时，已经开始的线程并不会结束，而是会继续运行，直到全部线程运行结束后parallel for 方法才会返回结果
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.Token.Register(() => { Loger.Logger.Information("文件路径刷去任务超时开始取消；"); });
            cts.CancelAfter(TimeSpan.FromSeconds(300));
            var option = new ParallelOptions();
            option.CancellationToken = cts.Token;

            var disklist = DiskCollection.Keys;

            try
            {
                ParallelLoopResult result = Parallel.ForEach(disklist, option, (disk) => { Refreshdisk(disk, cts.Token); });
                Loger.Logger.Information("finished Refresh, time is {0}", DateTime.Now);
            }
            catch (OperationCanceledException e)
            {
                Loger.Logger.Error("路径搜寻超过设定时间已被取消，请调查问题原因；");
            }

            GC.Collect();
        }
        public static void Refreshdisk(HardDisk disk, CancellationToken token)
        {
            // TODO：对发生状态变更的硬盘进行记录；
            DiskStatus pastStatus = disk.Status;
            try
            {
                var newTime = Directory.GetLastWriteTimeUtc(disk.OriginPath);
                // 查看目录文件夹是否更新；
                if (disk.LastSearchTime != newTime)
                {
                    HashSet<string> newPath = GetDiskPathCollection(disk);
                    if (!token.IsCancellationRequested)
                    {
                        PathContainerRefresh(disk, newPath);
                        Loger.Logger.Information("硬盘刷新完成 {0},{1}", disk.DiskName, disk.ParentPc.PcIp);
                    }
                    else
                    {
                        Loger.Logger.Information("任务取消不刷新硬盘 {0},{1}", disk.DiskName, disk.ParentPc.PcIp);
                    }
                    disk.LastSearchTime = newTime;
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
                // TODO: 检查线程是否取消,如果是取消后才进行至这一步，
                // 则不对硬盘状态进行任何的更改，
                // 应该记录该行为并调查为何路径查询耗费了过长的时间（通常单个硬盘返回结果的时间不超过1S，SVI因为文件数量较多也不应超过3S），
                // 最为重要的是当这个卡顿的时间远超过刷新路径的10或15min的周期时（通常是因为硬盘掉线），
                // 应设计避免超时的线程修改当前hashset的情况发生；
                //if (disk.Status != DiskStatus.OK)
                //{

                //}
                //if (disk.Status != pastStatus)
                //{
                //    string statusChangedInfo;
                //    if (disk.Status == DiskStatus.OK)
                //    {
                //        statusChangedInfo = "新添加硬盘；";
                //    }
                //    else
                //    {
                //        statusChangedInfo = "硬盘损坏；";
                //    }
                //    //DiskStatusChangedEventArgs newevent = new DiskStatusChangedEventArgs(disk, pastStatus, statusChangedInfo);
                //    //DiskStatusChangedEvent?.Invoke(null, newevent);
                //}
            }
        }
        /// <summary>
        /// 更新disk中filepath 的内容
        /// </summary>
        /// <param name="disk">要修改的disk object</param>
        /// <param name="path">新的路径hashset</param>
        static void PathContainerRefresh(HardDisk disk, HashSet<string> path)
        {
            // 防止多线程情况下，修改与查询发生内存冲突；
            if (DiskCollection.ContainsKey(disk))
            {
                lock (disk)
                {
                    DiskCollection[disk] = path;
                }
            }
            else
            {
                lock (DiskCollection)
                {
                    DiskCollection.Add(disk, path);
                }
            }
        }
        static public Dictionary<string,List<PanelPathContainer>> GetPanelPathList(IEnumerable<string> panelIdList)
        {
            Dictionary<string, List<PanelPathContainer>> collection = new Dictionary<string, List<PanelPathContainer>> ();
            foreach (var disk in DiskCollection)
            {
                lock (disk.Key)
                {
                    // diskcollection 初始化状态为Null，会存在大量的硬盘为null的情况，需要跳过这些硬盘；
                    if (disk.Value != null)
                    {
                        foreach (var panel in panelIdList)
                        {
                            if (disk.Value.Contains(panel))
                            {
                                if (collection.ContainsKey(panel))
                                {
                                    collection[panel].Add(new PanelPathContainer(panel, disk.Key.ParentPc, disk.Key.DiskName));
                                }
                                else
                                {
                                    collection.Add(panel, new List<PanelPathContainer>());
                                    collection[panel].Add(new PanelPathContainer(panel, disk.Key.ParentPc, disk.Key.DiskName));
                                }
                            }
                        }
                    }
                }
            }
            foreach (var item in panelIdList)
            {
                if (!collection.ContainsKey(item))
                {
                    collection.Add(item, null);
                }
            }
            return collection;
        }
        public static HashSet<string> GetDiskPathCollection(HardDisk disk)
        {
            /// 
            /// 从给定的硬盘中获取文件夹列表
            /// 已经验证过在多线程情况下直接获取string list 的速度和获取 string iteration并迭代的速度基本没有差别，主要取决与网速限制；
            /// 
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
