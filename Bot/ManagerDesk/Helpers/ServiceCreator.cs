using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LiteDbService;
using System.Configuration;
using ManagerDesk.Enums;

namespace ManagerDesk.Helpers
{
    public static class ServiceCreator
    {
        public static LiteCustomerService GetCustomerService()
        {
            var dbType =  ConfigurationManager.AppSettings.Get("DbType");
            if (dbType == DbTypes.TestDb.ToString())
                return new TestLiteCustomerService();
            else
                return new LiteCustomerService();
        }
        public static LiteManagerService GetManagerService()
        {
            var dbType = ConfigurationManager.AppSettings.Get("DbType");
            if (dbType == DbTypes.TestDb.ToString())
                return new TestLiteManagerService();
            else
                return new LiteManagerService();
        }
    }
}