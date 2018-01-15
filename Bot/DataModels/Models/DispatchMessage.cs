using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class DispatchMessage
    {
        public Guid Id { get; set; }

        public Guid DispatchId { get; set; }

        public long ChatId { get; set; }

        public string ExecutionResult { get; set; }

        public bool Send { get; set; }
    }
}