using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brains.Responces
{
    public class Responce
    {
        public long ChatId { get; set; }

        public string ResponceText { get; set; }

        public static Responce UnknownResponce(long chatId)
        {
            return new Responce { ChatId = chatId, ResponceText = "Извините, не понял вашей просьбы :(" };
        }
    }
}
