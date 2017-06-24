using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Exceptions
{

    [Serializable]
    public class TableNotFoundException : Exception
    {
        public TableNotFoundException() { }
        public TableNotFoundException(string message) : base(message) { }
        public TableNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected TableNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
