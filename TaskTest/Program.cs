using CoreClass;
using CoreClass.DICSEnum;
using CoreClass.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace TaskTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PerformanceCounterCategory pcg = new PerformanceCounterCategory("Network Interface");
            foreach (var instance in pcg.GetInstanceNames())
            {
                PerformanceCounter pcsent = new PerformanceCounter("Network Interface", "Bytes Sent/sec", instance);
                PerformanceCounter pcreceived = new PerformanceCounter("Network Interface", "Bytes Received/sec", instance);

                Console.WriteLine("name: {0}", instance);
                pcsent.NextValue();
                pcreceived.NextValue();
                Thread.Sleep(1000);
                Console.WriteLine("Bytes Sent: {0}", pcsent.NextValue() / 1024);
                Console.WriteLine("Bytes Received: {0}", pcreceived.NextValue() / 1024);
            }
        }
    }
    public static class FileManager
    {
        static List<PC> InsPCList = IpTransform.Name2IP(new Pcinfo[] { Pcinfo.AVI, Pcinfo.SVI });
        static List<HardDisk> DiskCollection = new List<HardDisk>();
        static Dictionary<HardDisk, HashSet<string>> PathContainer = new Dictionary<HardDisk, HashSet<string>>();

        static FileManager()
        {
            foreach (var disk in (Disk[])Enum.GetValues(typeof(Disk)))
            {
                foreach (var pc in InsPCList)
                {
                    DiskCollection.Add(new HardDisk(pc, disk));
                };
            };
        }
        public static void TEST()
        {
            Loger.Logger.Information("开始测试，开始时间为： {0}", DateTime.Now);
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.Token.Register(() => { Loger.Logger.Information("触发任务取消"); });
            cts.CancelAfter(TimeSpan.FromSeconds(70));
            var option = new ParallelOptions();
            option.CancellationToken = cts.Token;
            Loger.Logger.Information("start to refresh the file dict, time ： {0}", DateTime.Now);
            Parallel.For(0, DiskCollection.Count - 1,option, i => { Refreshdisk(DiskCollection[i], cts.Token); });
            Loger.Logger.Information("finished Refresh, time is {0}", DateTime.Now);
        }
        public static void RefreshFileList()
        {
            Loger.Logger.Information("start to refresh the file dict, time ： {0}", DateTime.Now);

            // 当任务取消时，已经开始的线程并不会结束，而是会继续运行，直到全部线程运行结束后parallel for 方法才会返回结果
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.Token.Register(() => { Loger.Logger.Information("文件路径刷去任务超时开始取消；"); });
            cts.CancelAfter(TimeSpan.FromSeconds(70));
            var option = new ParallelOptions();
            option.CancellationToken = cts.Token;

            try
            {
                Loger.Logger.Information("路径搜寻正常完成；");
                ParallelLoopResult result = Parallel.For(0, DiskCollection.Count - 1, option, i => { Refreshdisk(DiskCollection[i], cts.Token); });
            }
            catch (OperationCanceledException e)
            {

                Loger.Logger.Error("路径搜寻超过设定时间已被取消，请调查问题原因；");
            }

            Loger.Logger.Information("finished Refresh, time is {0}", DateTime.Now);
            GC.Collect();
        }
        public static void Refreshdisk(HardDisk disk, CancellationToken token) 
        {
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
                        Loger.Logger.Information("任务取消不刷新硬盘 {0},{1}", disk.DiskName,disk.ParentPc.PcIp);
                    }
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
                disk.lastErrorMessage = e.Message;
            }
            catch (DirectoryNotFoundException e)
            {
                // 硬盘损坏，或该路径下硬盘无实物存在，该硬盘路径无法通过远程访问的方式打开
                //FilePathLogClass.Logger.Debug(e.Message);
                disk.Status = DiskStatus.NotExist;
                disk.lastErrorMessage = e.Message;
            }
            catch (IOException e)
            {
                // 网络通信问题，网络无法链接到该计算机，可能是因为该计算连接交换机的网线出现了故障，或网卡断开，需要检查设备的硬件原因；
                //FilePathLogClass.Logger.Debug(e.Message);
                disk.Status = DiskStatus.ConnectError;
                disk.lastErrorMessage = e.Message;
            }
            catch (Exception e)
            {
                //FilePathLogClass.Logger.Error(e.Message);
                disk.Status = DiskStatus.ConnectError;
                disk.lastErrorMessage = e.Message;
            }
            finally
            {
                if (disk.Status != DiskStatus.OK)
                {
                    lock (disk) lock (PathContainer)
                        {
                            PathContainer.Remove(disk);
                        }
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
                }
            }
        }
        static void PathContainerRefresh(HardDisk disk, HashSet<string> path)
        {
            if (PathContainer.ContainsKey(disk))
            {
                lock (disk)
                {
                    PathContainer[disk] = path;
                }
            }
            else
            {
                lock (PathContainer)
                {
                    PathContainer.Add(disk, path);
                }
            }
        }
        public static HashSet<string> GetDiskPathCollection(HardDisk disk)
        {
            disk.Status = DiskStatus.OnSearch;

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
