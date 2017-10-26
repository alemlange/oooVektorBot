using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;

namespace DataModels.Configuration
{
    public class ConfigurationSettings
    {
        public struct AppKeys
        {
            public const string DbType = "DbType";
            public const string AccountId = "AccountId";
            public const string ExceptionPath = "ExceptionPath";
            public const string FilePath = "FilePath";
        }

        public static string DbType
        {
            get { return ConfigurationManager.AppSettings[AppKeys.DbType]; }
        }

        public static string ExceptionPath
        {
            get { return ConfigurationManager.AppSettings[AppKeys.ExceptionPath]; }
        }

        public static string FilePath
        {
            get { return ConfigurationManager.AppSettings[AppKeys.FilePath]; }
        }

        public static Guid AccountId
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppKeys.AccountId];
                Guid guidValue;
                if (setting != null && Guid.TryParse(setting, out guidValue))
                {
                    return guidValue;
                }
                else
                {
                    return Guid.Empty;
                }
            }
        }
    }
}
