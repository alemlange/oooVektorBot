using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDbService.Interfaces;
using DataModels;
using LiteDB;

namespace LiteDbService
{
    public class TestLiteCustomerService : LiteCustomerService
    {
        public TestLiteCustomerService() : base("")
        {

        }

        public override string CurrentDb
        {
            get
            {
                if (string.IsNullOrEmpty(_currentDb))
                {
                    _currentDb = @"C:\db\TestData.db";
                }
                return _currentDb;
            }
        }
    }
}
