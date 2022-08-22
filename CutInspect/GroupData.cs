using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutInspect
{
    public class GroupData
    {
        public string EqpName;
        public List<InspectItem> InspectItems;
        public int finishedItem { get; set; }
        public int totalItems { get; set; }

        public GroupData(JObject data)
        {
            EqpName = data.GetValue("").ToString();
            InspectItems = new List<InspectItem>();
        }
    }
}
