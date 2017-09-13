using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels;
using LiteDbService.Interfaces;
using LiteDB;

namespace LiteDbService
{
    public class TestLiteManagerService : LiteManagerService
    {
        public TestLiteManagerService() : base("")
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
