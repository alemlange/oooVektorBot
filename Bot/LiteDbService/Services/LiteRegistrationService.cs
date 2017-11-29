using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using DataModels;
using DataModels.Enums;

namespace LiteDbService
{
    public sealed class LiteRegistrationService
    {
        private string _currentDb { get; set; }
        
        private string CurrentDb
        {
            get
            {
                if (string.IsNullOrEmpty(_currentDb))
                {
                    _currentDb = @"C:\db\Accounts.db";
                }
                return _currentDb;
            }
        }

        public void CreateAccount(ManagerAccount account)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<ManagerAccount>("ManagerAccounts");
                col.Insert(account);
            }
        }

        public Config CreateConfig(Guid accountId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Config>("Configs");
                var config = new Config { Id = Guid.NewGuid(), AccountId = accountId, TelegramBotLocation = "http://localhost:8086/" };

                col.Insert(config);
                return config;
            }
        }

        public ManagerAccount FindAccount(string login)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<ManagerAccount>("ManagerAccounts");
                return col.Find(o => o.Login == login).FirstOrDefault();
            }
        }

        public ManagerAccount FindAccount(Guid accountId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<ManagerAccount>("ManagerAccounts");
                return col.Find(o => o.Id == accountId).FirstOrDefault();
            }
        }

        public Config FindConfig(Guid accId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Config>("Configs");
                return col.Find(o => o.AccountId == accId).FirstOrDefault();
            }
        }

        public void UpdateConfig(Config config)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Config>("Configs");
                col.Update(config);
            }
        }
    }
}
