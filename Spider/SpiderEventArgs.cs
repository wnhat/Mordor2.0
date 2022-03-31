using CoreClass;
using CoreClass.DICSEnum;
using CoreClass.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spider
{
    public class SearchEventArgs : EventArgs
    {
        public SearchEventArgs(DateTime message)
        {
            SearchEventDate = message;
        }
        public DateTime SearchEventDate { get; set; }
    }
    public class AddNewCellLogEventArgs : EventArgs
    {
        public AddNewCellLogEventArgs(PanelInspectHistory info)
        {
            panel = info;
        }
        public PanelInspectHistory panel { get; set; }
    }
    public class AddNewResultFileEventArgs : EventArgs
    {
        public AddNewResultFileEventArgs(PanelInspectHistory info)
        {
            panel = info;
        }
        public PanelInspectHistory panel { get; set; }
    }
    public class DiskStatusChangedEventArgs : EventArgs
    {
        public DiskStatusChangedEventArgs(HardDisk diskinfo,DiskStatus paststatus, string info)
        {
            disk = diskinfo;
            Info = info;
        }
        public HardDisk disk { get; set; }
        public string Info { get; set; }
        public DiskStatus PastStatus { get; set; }
    }
}
