using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class Dispatch
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        public string Host { get; set; }

        public string Name { get; set; }

        public string Message { get; set; }

        public string ExecutionResult { get; set; }

        public bool Done { get; set; }
    }
}