using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataModels;
using System.ComponentModel.DataAnnotations;

namespace ManagerDesk.ViewModels
{
    public class DispatchViewModel
    {
        public Guid Id { get; set; }

        public string Host { get; set; }

        public string Name { get; set; }

        public string Message { get; set; }

        public bool Done { get; set; }
    }
}