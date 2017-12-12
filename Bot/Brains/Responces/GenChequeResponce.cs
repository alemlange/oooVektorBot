using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels;

namespace Brains.Responces
{
    public class GenChequeResponce: Responce
    {
        public Cheque Invoice { get; set; }

        public bool InvoiceReady { get; set; }
    }
}
