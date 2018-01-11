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

        public string Host { get; set; }

        public string Name { get; set; }

        public string Message { get; set; }

        public int Done { get; set; }
    }
}