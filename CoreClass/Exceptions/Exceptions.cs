using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreClass.Exceptions
{

    [Serializable]
    public class LogSpiderException : Exception
    {
        public LogSpiderException() { }
        public LogSpiderException(string message) : base(message) { }
        public LogSpiderException(string message, Exception inner) : base(message, inner) { }
        protected LogSpiderException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
