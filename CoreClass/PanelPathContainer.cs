using CoreClass.DICSEnum;
using CoreClass.Model;
using System;

namespace CoreClass
{
    public class PanelPathContainer
    {
        public string PanelId { get; set; }
        public PC PcInfo;
        public Disk diskName;
        public PanelPathContainer(string panel_id, PC pc, Disk diskName)
        {
            PanelId = panel_id;
            PcInfo = pc;
            this.diskName = diskName;
        }
        public string EqName { get { return PcInfo.EqName; } }
        public int EqId { get { return PcInfo.EqId; } }
        public string DiskName { get { return diskName.ToString(); } }
        public Pcinfo PcName { get { return PcInfo.PcName; } }
        public string PcIp { get { return PcInfo.PcIp; } }
        public string OriginImagePath
        {
            get
            {   // \\172.16.180.83\NetworkDrive\F_Drive\Defect Info\Origin
                string returnstring = "\\\\" + PcInfo.PcIp + "\\NetworkDrive\\" + DiskName + "\\Defect Info\\Origin\\" + PanelId;
                return returnstring;
            }
        }
        public string ResultPath
        {
            get
            {   // \\172.16.180.83\NetworkDrive\F_Drive\Defect Info\Result
                string returnstring = "\\\\" + PcInfo.PcIp + "\\NetworkDrive\\" + DiskName + "\\Defect Info\\Result\\" + PanelId;
                return returnstring;
            }
        }
        public override string ToString()
        {
            return PanelId;
        }
    }
}