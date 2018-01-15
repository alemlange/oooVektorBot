using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Brains;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using LiteDbService;
using LiteDbService.Helpers;
using Clients;

namespace DispatchesService
{
    class Program
    {
        private static LiteDispatchesService _dispService = ServiceCreator.GetDispatchesService();

        static void Main(string[] args)
        {
            while (true)
            {
                var dispatches = _dispService.GetAllActiveDispatches();

                foreach (var disp in dispatches)
                {
                    Thread dispThread = new Thread(() => PrepareDispatch(disp.Id, disp.Host, disp.Message));
                    dispThread.Start();
                }

                Thread.Sleep(300000);
            }
        }

        public static void PrepareDispatch(Guid dispatchId, string host, string message)
        {
            var botClient = new BotClient(host);
            var messages = _dispService.GetDispatchMessages(dispatchId);

            if (messages.Count > 0)
            {
                foreach (var msg in messages)
                {
                    string res = "";
                    try
                    {
                        res = botClient.SendNotification(msg.ChatId, message);
                        _dispService.SetDispatchMessageDone(msg.Id, true, res);
                    }
                    catch (Exception ex)
                    {
                        res = ex.Message;
                        _dispService.SetDispatchMessageDone(msg.Id, false, res);
                    }

                    Thread.Sleep(2000);
                }

                _dispService.SetDispatchDone(dispatchId, true, "Ok");
            }
            else
            {
                _dispService.SetDispatchDone(dispatchId, false, "В рассылке отсутствуют сообщения!");
            }
        }
    }
}