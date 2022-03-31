using CoreClass;
using CoreClass.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spider
{
    [Serializable]
    public class InspectionFileSearchException : System.Exception
    {
        PC pC;
        HardDisk disk;
        DateTime errortime = DateTime.Now;
        public InspectionFileSearchException(string message, PC pC, HardDisk disk) : base(message)
        {
            this.pC = pC;
            this.disk = disk;
        }
        public InspectionFileSearchException(string message, System.Exception inner) : base(message, inner) { }
        protected InspectionFileSearchException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
