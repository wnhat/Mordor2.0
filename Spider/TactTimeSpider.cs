using CoreClass.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spider
{
    public class TactTimeSpider
    {
        public Queue<LogMainTact> PanelIdQueue = new Queue<LogMainTact>();
        public PC mainpc;

        public TactTimeSpider(PC mainpc)
        {
            this.mainpc = mainpc;
        }

    }
}
