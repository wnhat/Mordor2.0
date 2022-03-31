using CoreClass.DICSEnum;
using CoreClass.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoreClass
{
    public class HardDisk
    {
        public PC ParentPc;
        public Disk DiskName;
        public DiskStatus Status = DiskStatus.Unchecked;
        public string lastErrorMessage = null;
        public DateTime LastSearchTime = DateTime.UtcNow;
        public HardDisk(PC parentPc, Disk diskName)
        {
            ParentPc = parentPc;
            DiskName = diskName;
        }
        public string DefectInfoPath
        {
            get
            {
                return Path.Combine("\\\\", ParentPc.PcIp, "NetworkDrive", DiskName.ToString(), "Defect Info");
            }
        }
        public string OriginPath
        {
            get
            {
                return Path.Combine("\\\\", ParentPc.PcIp, "NetworkDrive", DiskName.ToString(), "Defect Info", "Origin");
            }
        }
        public string ResultPath
        {
            get
            {
                return Path.Combine("\\\\", ParentPc.PcIp, "NetworkDrive", DiskName.ToString(), "Defect Info", "Result");
            }
        }

    }
}
