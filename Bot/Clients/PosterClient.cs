using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Net;
using System.IO;
using DataModels;

namespace Clients
{
    public class PosterClient
    {
        private string _userLogin { get; set; }

        private string _userPassword { get; set; }

        private string _userToken { get; set; }

        public PosterClient(string userLogin, string userPassword)
        {
            _userLogin = userLogin;
            _userPassword = userPassword;

            _userToken = GetToken();
        }

        private string GetToken()
        {
            var client = new WebClient();

            client.Headers["Content-type"] = "application/json";
            client.Encoding = Encoding.UTF8;

            return client.DownloadString("https://iiko.biz:9900/" + "api/0/auth/access_token?user_id=" + _userLogin+ "&user_secret="+ _userPassword);
        }

        public string GetOrganizations()
        {
            var client = new WebClient();

            client.Headers["Content-type"] = "application/json";
            client.Encoding = Encoding.UTF8;

            return client.DownloadString("https://iiko.biz:9900/" + "api/0/organization/list?access_token=" + _userToken + "&request_timeout=00%3A02%3A00");
        }

        public string GetDishes(Guid orgId)
        {
            var client = new WebClient();

            client.Headers["Content-type"] = "application/json";
            client.Encoding = Encoding.UTF8;

            return client.DownloadString("https://iiko.biz:9900/" + "api/0/nomenclature/" + orgId.ToString() + "?access_token=" + _userToken + "&organizationId="+ orgId.ToString());
        }

        public string SendOrder(List<Dish> order)
        {
            var client = new WebClient();

            client.Headers["Content-type"] = "application/json";
            client.Encoding = Encoding.UTF8;

            var serializer = new DataContractJsonSerializer(typeof(List<Dish>));
            MemoryStream mem = new MemoryStream();
            serializer.WriteObject(mem, order);

            var data = Encoding.UTF8.GetString(mem.ToArray(), 0, (int)mem.Length);

            var responce = client.UploadString("https://iiko.biz:9900/" + "api/0/orders/add?&access_token="+_userToken+ "&requestTimeout=10000", "POST", data);

            return responce;
        }
    }
}
