using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreClass.DICSEnum;

namespace CoreClass.Model
{
    public class LogMainTact
    {
        public DateTime InspDate { get; set; }
        public string ModelID { get; set; }
        public decimal TactTime { get; set; }
        public int CellCount { get; set; }
        public OperationID OperationID { get; set; }
        public string LineMode { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
    }
}
