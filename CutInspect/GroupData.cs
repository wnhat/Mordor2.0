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
        public int finishedItem { get 
            {
                int count = 0;
                foreach (var item in InspectItems)
                {
                    if (item.status == 1)
                    {
                        count++;
                    }
                }
                return count;
            } }
        public int totalItems { get { return InspectItems.Count; } }

        public GroupData(string EqpName,List<InspectItem> items)
        {
            this.EqpName = EqpName;
            this.InspectItems = new List<InspectItem>();
        }
    }
}
