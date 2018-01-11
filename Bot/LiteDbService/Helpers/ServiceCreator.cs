using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LiteDbService;
using LiteDbService.Enums;
using DataModels.Configuration;

namespace LiteDbService.Helpers
{
    public static class ServiceCreator
    {
        public static LiteCustomerService GetCustomerService(string userName)
        {
            var dbType = ConfigurationSettings.DbType;
            if (dbType == DbTypes.TestDb.ToString())
                return new TestLiteCustomerService();
            else
                return new LiteCustomerService(userName);
        }

        public static LiteManagerService GetManagerService(string userName)
        {
            var dbType = ConfigurationSettings.DbType;
            if (dbType == DbTypes.TestDb.ToString())
                return new TestLiteManagerService();
            else
                return new LiteManagerService(userName);
        }

        public static LiteRegistrationService GetRegistrationService()
        {
            return new LiteRegistrationService();
        }

        public static LiteDispatchesService GetDispatchesService()
        {
            return new LiteDispatchesService();
        }
    }
}