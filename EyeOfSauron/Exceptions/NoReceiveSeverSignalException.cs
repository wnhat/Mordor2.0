using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeOfSauron.Exceptions
{
    public class NoReceiveSeverSignalException : Exception
    {
        public NoReceiveSeverSignalException() { }
        public NoReceiveSeverSignalException(string message) : base(message) { }
        public NoReceiveSeverSignalException(string message, Exception inner) : base(message, inner) { }
        protected NoReceiveSeverSignalException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
