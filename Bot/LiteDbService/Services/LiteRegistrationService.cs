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

        public ManagerAccount FindAccount(string login)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<ManagerAccount>("ManagerAccounts");
                return col.Find(o => o.Login == login).FirstOrDefault();
            }
        }
    }
}
