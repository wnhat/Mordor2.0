using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreClass.LogSpider
{
    interface ISpiderInterface
    {
        public void GetLog();
        public void Insert2DB();
        public void Search();
    }
}
