using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brains.DbService.Interfaces;
using Brains.Models;

namespace Brains.DbService
{
    public class DbServiceSql : IDb
    {
        public Menu GetMenu()
        {
            return new Menu();
        }
    }
}
