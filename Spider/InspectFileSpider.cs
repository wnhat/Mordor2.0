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
                PathContainer.Add(disk, path);
            }
        }
        static public List<PanelPathContainer> GetPanelPath(string panelId)
        {
            List<PanelPathContainer> pathlist = new List<PanelPathContainer>();
            foreach (var disk in PathContainer)
            {
                lock (disk.Key) lock (PathContainer)
                {
                    if (disk.Value.Contains(panelId))
                    {
                        pathlist.Add(new PanelPathContainer(panelId, disk.Key.ParentPc, disk.Key.DiskName));
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
                lock (disk.Key)
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

    public class InspectFileSpider
    {
        Dictionary<PC, Dictionary<HardDisk, HashSet<string>>> pathcontainer = new Dictionary<PC, Dictionary<HardDisk, HashSet<string>>>();
        public event EventHandler<DiskStatusChangedEventArgs> DiskStatusChangedEvent;

        public InspectFileSpider(List<PC> inspectPc)
        {
            foreach (var item in inspectPc)
            {
                pathcontainer.Add(item, new Dictionary<HardDisk, HashSet<string>>());
            }
        }
        public PC[] _PCinsearch { get { return pathcontainer.Keys.ToArray(); } }
        public Task RefreshAsync(CancellationToken token)
        {
            Task[] tasks = new Task[_PCinsearch.Length];

            Task SearchParallel = Task.Run(() => { Parallel.ForEach(pathcontainer.Keys, i => { RefreshFileList(i); }); });
            
            return SearchParallel;
        }
        public void Refresh()
        {
//            ParallelOptions parallelOptions = new ParallelOptions();

//            Task[] tasks = new Task[actionsCopy.Length];

//            // One more check before we begin...
//            parallelOptions.CancellationToken.ThrowIfCancellationRequested();

//            // Invoke all actions as tasks.  Queue N-1 of them, and run 1 synchronously.
//            for (int i = 1; i < tasks.Length; i++)
//            {
//                tasks[i] = Task.Factory.StartNew(actionsCopy[i], parallelOptions.CancellationToken, TaskCreationOptions.None,
//                                                 parallelOptions.EffectiveTaskScheduler);
//            }
//            tasks[0] = new Task(actionsCopy[0], parallelOptions.CancellationToken, TaskCreationOptions.None);
//            tasks[0].RunSynchronously(parallelOptions.EffectiveTaskScheduler);

//            // Now wait for the tasks to complete.  This will not unblock until all of
//            // them complete, and it will throw an exception if one or more of them also
//            // threw an exception.  We let such exceptions go completely unhandled.
//            try
//            {
//#pragma warning disable CA1416 // Validate platform compatibility, issue: https://github.com/dotnet/runtime/issues/44605
//                Task.WaitAll(tasks);
//#pragma warning restore CA1416
//            }
//            catch (AggregateException aggExp)
//            {
//                // see if we can combine it into a single OCE. If not propagate the original exception
//                //ThrowSingleCancellationExceptionOrOtherException(aggExp.InnerExceptions, parallelOptions.CancellationToken, aggExp);
//            }


        }
        public HashSet<string> GetDiskPathCollection(HardDisk disk)
        {
            disk.Status = DiskStatus.Unchecked;
            try
            {
                var newTime = Directory.GetLastWriteTimeUtc(disk.OriginPath);
                if (!(disk.LastSearchTime == newTime))
                {
                    string[] image_directory_list = Directory.GetDirectories(disk.OriginPath);
                    string[] result_directory_list = Directory.GetDirectories(disk.ResultPath);
                    disk.Status = DiskStatus.OK;
                    var intersectlist = Enumerable.Intersect(image_directory_list, result_directory_list, new StringPathCompare());
                    HashSet<string> panelIdList = new HashSet<string>();
                    foreach (var item in intersectlist)
                    {
                        panelIdList.Add(Path.GetFileName(item));
                    }
                    disk.LastSearchTime = newTime;
                    return panelIdList;
                }
                else
                {
                    return null;
                }
            }
            catch (UnauthorizedAccessException e)
            {
                // 硬盘损坏，该硬盘路径无法通过远程访问的方式打开
                //FilePathLogClass.Logger.Debug(e.Message);
                disk.Status = DiskStatus.ConnectError;
                disk.lastErrorMessage = e.Message;
                return null;
            }
            catch (DirectoryNotFoundException e)
            {
                // 硬盘损坏，或该路径下硬盘无实物存在，该硬盘路径无法通过远程访问的方式打开
                //FilePathLogClass.Logger.Debug(e.Message);
                disk.Status = DiskStatus.NotExist;
                disk.lastErrorMessage = e.Message;
                return null;
            }
            catch (IOException e)
            {
                // 网络通信问题，网络无法链接到该计算机，可能是因为该计算连接交换机的网线出现了故障，或网卡断开，需要检查设备的硬件原因；
                //FilePathLogClass.Logger.Debug(e.Message);
                disk.Status = DiskStatus.ConnectError;
                disk.lastErrorMessage = e.Message;
                return null;
            }
            catch (Exception e)
            {
                //FilePathLogClass.Logger.Error(e.Message);
                disk.Status = DiskStatus.ConnectError;
                disk.lastErrorMessage = e.Message;
                return null;
            }
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

    public void RefreshCurrentAccesableDisk(PC pc)
        {
            var currentDiskList = pathcontainer[pc].Keys;
            var lastAccesableDiskName = from _disk in currentDiskList
                                        select _disk.DiskName;
            var allDiskName = (Disk[])Enum.GetValues(typeof(Disk));
            foreach (var disk in currentDiskList)
            {
                // 检查上次刷新完成的硬盘可访问性，当可访问性发生变化时抛出硬盘损坏（Event）；
                try
                {
                    bool result = Task.Run(() => { Directory.GetDirectories(disk.DefectInfoPath); }).Wait(5000);
                }
                catch (Exception e)
                {
                    DeleteErrorDisk(disk);
                    string errorstring = String.Format("{0}设备 硬盘：{1}，发生损坏；上次刷新成功的时间为：{2};", disk.ParentPc.EqName, disk.DiskName, disk.LastSearchTime);
                    //var neweventargs = new DiskStatusChangedEventArgs(disk,errorstring);
                    //DiskStatusChangedEvent?.Invoke(this, neweventargs);
                }
            }
            var newtry = allDiskName.Except(lastAccesableDiskName);
            foreach (var item in newtry)
            {
                // 检查是否存在能够新加入的磁盘；
                HardDisk newdisk = new HardDisk(pc, item);
                try
                {
                    Directory.GetDirectories(newdisk.DefectInfoPath);
                    AddNewDisk(newdisk);
                    string infostring = String.Format("新添加 {0} 设备 硬盘：{1} ", newdisk.ParentPc.EqName, newdisk.DiskName);
                    //var neweventargs = new DiskStatusChangedEventArgs(newdisk, infostring);
                    //DiskStatusChangedEvent?.Invoke(this, neweventargs);
                }
                catch(Exception e)
                {

                }
            }
        }
        string[] checkDiskAndR(string path)
        {
            try
            {
                var getstask = Task.Run(() => Directory.GetDirectories(path));
                while (getstask.Status == TaskStatus.WaitingToRun)
                {
                    Thread.Sleep(1000);
                }
                if (getstask.Wait(5000))
                {
                    return getstask.Result;
                }
                else
                {
                    throw new ApplicationException("");
                }
            }
            catch (Exception)
            {
                throw;
            }
            
        }
        void DeleteErrorDisk(HardDisk disk)
        {
            lock (disk.ParentPc)
            {
                pathcontainer[disk.ParentPc].Remove(disk);
            }
        }
        void AddNewDisk(HardDisk disk)
        {
            lock (disk.ParentPc)
            {
                pathcontainer[disk.ParentPc].Add(disk, new HashSet<string>());
            }
        }
        public void RefreshFileList(PC pC)
        {
            Loger.Testlogger.Information("{0} 搜索开始；", pC.PcIp);

            int SearchWaitTime;  // 设定单独硬盘查找超时放弃的时间 millisecond;
            if (pC.PcName == Pcinfo.AVI)
            {
                SearchWaitTime = 10000;
            }
            else
            {
                SearchWaitTime = 20000;
            }

            var seachDiskTask = Task.Run(() => { RefreshCurrentAccesableDisk(pC); });
            while (seachDiskTask.Status != TaskStatus.Running)
            {
                Thread.Sleep(1000);
            }
            if (seachDiskTask.Wait(20000))
            {
                var dic = pathcontainer[pC];
                foreach (var disk in dic)
                {
                    var result = GetDiskPathCollection(disk.Key);
                    if (result != null)
                    {
                        lock (pC)
                        {
                            dic[disk.Key] = result;
                        }
                    }
                    else
                    {
                        // todo: Log if get hashset error;
                    }
                }
            }
            else
            {
                Loger.Testlogger.Information("{0} 刷新硬盘list出错 ", pC.PcIp);
                //throw new Exception("查找硬盘报错");
            }
            Loger.Testlogger.Information("{0} 搜索完成",pC.PcIp);
        }
        public void GetPanelPathList(string[] panelIdList, PanelPathManager newManager)
        {
            foreach (var pc in pathcontainer.Keys)
            {
                var pcDic = pathcontainer[pc];
                lock (pc)
                {
                    foreach (var disk in pcDic.Keys)
                    {
                        var idHashSet = pcDic[disk];
                        // todo: use intersect;
                        foreach (var panel in panelIdList)
                        {
                            if (idHashSet.Contains(panel))
                            {
                                var newpaenlpath = new PanelPathContainer(panel, disk.ParentPc, disk.DiskName);
                                newManager.AddPanelPath(newpaenlpath);
                            }
                        }
                    }
                }
            }
        }
        public void SetPanelPathList(PC pC,Dictionary<HardDisk,HashSet<string>> value)
        {
            lock (pC)
            {
                pathcontainer[pC] = value;
            }
        }
    }
}
