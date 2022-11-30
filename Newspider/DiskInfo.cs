using CoreClass.DICSEnum;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newspider
{
    public class DiskInfo
    {
        public string PcIp;
        public Disk DiskName;
        public DateTime LastSearchTime = DateTime.UtcNow;
        public DiskStatus Status = DiskStatus.Unchecked;
        public DiskInfo(HashEntry[] info)
        {
            foreach (var item in info)
            {
                //if (item.Name == "pcip")
                //{
                //    PcIp = item.Value;
                //}
                //else if (item.Name == "")
                //{
                //}
                switch (item.Name)
                {
                    case "PcIp":
                        PcIp = item.Value;
                        break;
                    case "DiskName":
                        DiskName = Enum.Parse<Disk>(item.Value);
                        break;
                    case "LastSearchTime":
                        LastSearchTime = ((IConvertible)item.Value).ToDateTime(null);
                        break;
                    case "Status":
                        Status = Enum.Parse<DiskStatus>(item.Value);
                        break;
                    default:
                        break;
                }
            }
        }
        public DiskInfo(string pcip,Disk disk)
        {
            PcIp = pcip;
            DiskName = disk;
        }
        public string RedisInfoKey
        {
            get
            {
                return "path:info:" + PcIp + ":" + DiskName.ToString();
            }
        }
        public HashEntry[] RedisInfoValue
        {
            get
            {
                var result = new HashEntry[4];
                result[0] = new HashEntry("PcIp", PcIp);
                result[1] = new HashEntry("DiskName", DiskName.ToString());
                result[2] = new HashEntry("LastSearchTime", LastSearchTime.ToString());
                result[3] = new HashEntry("status", Status.ToString());
                //for (int i = 0; i < result.Length; i++)
                //{
                //    result[i] = new HashEntry(RedisInfoKey, result[i].Value);
                //}
                return result;
            }
        }
        public string RedisPathKey
        {
            get
            {
                return "path:path:" + PcIp + ":" + DiskName.ToString();
            }
        }
        public string DefectInfoPath
        {
            get
            {
                return Path.Combine("\\\\", PcIp, "NetworkDrive", DiskName.ToString(), "Defect Info");
            }
        }
        public string OriginPath
        {
            get
            {
                return Path.Combine("\\\\", PcIp, "NetworkDrive", DiskName.ToString(), "Defect Info", "Origin");
            }
        }
        public string ResultPath
        {
            get
            {
                return Path.Combine("\\\\", PcIp, "NetworkDrive", DiskName.ToString(), "Defect Info", "Result");
            }
        }
    }
}
