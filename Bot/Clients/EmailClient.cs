using System.Text;
using System.Net;
using DataModels.Email;
using DataModels.Configuration;
using System.Runtime.Serialization.Json;
using System.IO;

namespace Clients
{
    public class EmailClient
    {
        public void Send(string to, string message, string title)
        {
            var client = new WebClient();

            client.Headers["Content-type"] = "application/json";
            client.Encoding = Encoding.UTF8;

            var request = new EmailRequest { Body = message, Subject=title, To = to};

            var serializer = new DataContractJsonSerializer(typeof(EmailRequest));
            MemoryStream mem = new MemoryStream();
            serializer.WriteObject(mem, request);

            var data = Encoding.UTF8.GetString(mem.ToArray(), 0, (int)mem.Length);

            var responce = client.UploadString(ConfigurationSettings.EmailService + "Email/", "POST", data);
        }
    }
}
