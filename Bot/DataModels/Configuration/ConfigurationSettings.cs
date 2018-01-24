using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

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
            public const string SMTPServer = "SMTPServer";
            public const string Mail = "Mail";
            public const string Password = "Password";
            public const string Port = "Port";
            public const string EmailService = "EmailService";
            public const string TestLogin = "TestLogin";
            public const string TestPassword = "TestPassword";
        }

        public static string EmailService
        {
            get { return ConfigurationManager.AppSettings[AppKeys.EmailService]; }
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

        public static string SMTPServer
        {
            get { return ConfigurationManager.AppSettings[AppKeys.SMTPServer]; }
        }

        public static string Mail
        {
            get { return ConfigurationManager.AppSettings[AppKeys.Mail]; }
        }

        public static string Password
        {
            get { return ConfigurationManager.AppSettings[AppKeys.Password]; }
        }

        public static string TestLogin
        {
            get { return ConfigurationManager.AppSettings[AppKeys.TestLogin]; }
        }

        public static string TestPassword
        {
            get { return ConfigurationManager.AppSettings[AppKeys.TestPassword]; }
        }

        public static int Port
        {
            get
            {
                var setting = ConfigurationManager.AppSettings[AppKeys.Port];
                int intValue;

                if (setting != null && int.TryParse(setting, out intValue))
                {
                    return intValue;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}