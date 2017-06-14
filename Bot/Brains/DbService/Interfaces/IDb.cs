using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brains.Models;

namespace Brains.DbService.Interfaces
{
    interface IDb
    {
        Menu GetMenu();
    }
}
