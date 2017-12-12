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
                var keyNotGenerated = true;
                var botRndPart = "";
                var col = db.GetCollection<Config>("Configs");

                while (keyNotGenerated)
                {
                    var random = new Random();
                    var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
                    botRndPart = new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());

                    var matches = col.Find(o => o.TelegramBotLocation.Contains(botRndPart));

                    if (!matches.Any())
                        keyNotGenerated = false;
                }

                var config = new Config { Id = Guid.NewGuid(), AccountId = accountId, TelegramBotLocation = "https://" + botRndPart + ".karhouse.org/", BotKey = botRndPart + ".karhouse.org" };

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

        public Guid? AccountIdByHost(string uri)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Config>("Configs");
                var config = col.Find(o => o.TelegramBotLocation.Contains(uri)).FirstOrDefault();

                if(config != null)
                    return config.AccountId;
                else
                    return null;
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
