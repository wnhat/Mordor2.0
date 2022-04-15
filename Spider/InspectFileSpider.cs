using CoreClass.DICSEnum;
using CoreClass.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using CoreClass;

namespace Spider
{
    public static class FileManager
    {
        static List<PC> InsPCList = IpTransform.Name2IP(new Pcinfo[] { Pcinfo.AVI, Pcinfo.SVI});
        static List<HardDisk> DiskCollection = new List<HardDisk>();
        static Dictionary<HardDisk, HashSet<string>> PathContainer = new Dictionary<HardDisk, HashSet<string>>();

        public static event EventHandler<DiskStatusChangedEventArgs> DiskStatusChangedEvent;

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
        public static void RefreshFileList()
        {
            Loger.Logger.Information("start to refresh the file dict, time ： {0}", DateTime.Now);

            Task looptask = Task.Run(() => { Parallel.For(0, DiskCollection.Count - 1, i => { Refreshdisk(DiskCollection[i]); }); });
            if (looptask.Wait(120000))
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
                    PathContainerRefresh(disk, newPath);
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
                    lock (disk) lock(PathContainer)
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
                    DiskStatusChangedEventArgs newevent = new DiskStatusChangedEventArgs(disk,pastStatus, statusChangedInfo);
                    DiskStatusChangedEvent?.Invoke(null,newevent);
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
        static public List<PanelPathContainer> GetPanelPath(string panelId)
        {
            List<PanelPathContainer> pathlist = new List<PanelPathContainer>();
            // TODO: 当pathcontainer更新时，未完成的更新操作会阻断正在进行的enumration操作，导致部分ID查找不到；
            lock (PathContainer)
            {
                foreach (var disk in PathContainer)
                {
                    lock (disk.Key)
                    {
                        if (disk.Value.Contains(panelId))
                        {
                            pathlist.Add(new PanelPathContainer(panelId, disk.Key.ParentPc, disk.Key.DiskName));
                        }
                    }
                }
            }
            if (pathlist.Count == 0)
            {
                return null;
            }
            return pathlist;
        }
        static public PanelPathManager GetPanelPathList(IEnumerable<string> panelIdList)
        {
            PanelPathManager newManager = new PanelPathManager();
            foreach (var disk in PathContainer)
            {
                lock (disk.Key) lock (PathContainer)
                {
                    foreach (var panel in panelIdList)
                    {
                        if (disk.Value.Contains(panel))
                        {
                            newManager.AddPanelPath(new PanelPathContainer(panel, disk.Key.ParentPc, disk.Key.DiskName));
                        }
                    }
                }
            }
            foreach (var item in panelIdList)
            {
                if (!newManager.PathDict.ContainsKey(item))
                {
                    newManager.PathDict.Add(item, null);
                }
            }
            return newManager;
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
