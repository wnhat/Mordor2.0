using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutInspect.Model
{
    public class GroupData
    {
        public string EqpName;
        public List<InspectItem> InspectItems;
        public int FinishedItem 
        {
            get 
            {
                int count = 0;
                foreach (var item in InspectItems)
                {
                    if (item.Status != null)
                    {
                        count++;
                    }
                }
                return count;
            }
        }
        public int TotalItems { get { return InspectItems.Count; } }

        public GroupData(string EqpName,List<InspectItem> items)
        {
            this.EqpName = EqpName;
            this.InspectItems = items;
        }
    }
}
