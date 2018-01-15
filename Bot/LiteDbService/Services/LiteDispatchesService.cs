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
    public sealed class LiteDispatchesService
    {
        private string _currentDb { get; set; }
        
        private string CurrentDb
        {
            get
            {
                if (string.IsNullOrEmpty(_currentDb))
                {
                    _currentDb = @"C:\db\Dispatches.db";
                }
                return _currentDb;
            }
        }

        public void CreateDispatch(Dispatch dispatch)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Dispatch>("Dispatches");
                col.Insert(dispatch);
            }
        }

        public void CreateDispatchMessage(DispatchMessage dispatchMessage)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<DispatchMessage>("DispatchMessages");
                col.Insert(dispatchMessage);
            }
        }

        public List<Dispatch> GetAllActiveDispatches()
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Dispatch>("Dispatches");
                return col.Find(d => d.Done == false).ToList();
            }
        }

        public List<Dispatch> GetActiveDispatches(Guid accountId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Dispatch>("Dispatches");
                return col.Find(d => d.AccountId == accountId && d.Done == false).ToList();
            }
        }

        public List<Dispatch> GetInActiveDispatches(Guid accountId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Dispatch>("Dispatches");
                return col.Find(d => d.AccountId == accountId && d.Done).ToList();
            }
        }

        public List<DispatchMessage> GetDispatchMessages(Guid dispatchId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<DispatchMessage>("DispatchMessages");
                return col.Find(d => d.DispatchId == dispatchId && d.Send == false).ToList();
            }
        }

        public void SetDispatchMessageDone(Guid messageId, bool done, string execResult)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<DispatchMessage>("DispatchMessages");
                var msg = col.Find(d => d.Id == messageId).FirstOrDefault();

                if (msg != null)
                {
                    msg.Send = done;
                    msg.ExecutionResult = execResult;
                    col.Update(msg);
                }
            }
        }

        public void SetDispatchDone(Guid dispatchId, bool done, string execResult)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Dispatch>("Dispatches");
                var disp = col.Find(d => d.Id == dispatchId).FirstOrDefault();

                if (disp != null)
                {
                    disp.Done = done;
                    disp.ExecutionResult = execResult;
                    col.Update(disp);
                }
            }
        }
    }
}