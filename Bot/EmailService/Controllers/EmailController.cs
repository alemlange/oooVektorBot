using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Configuration;
using System.Web.Http;
using DataModels.Configuration;

namespace EmailService.Controllers
{
    public class EmailController : ApiController
    {
        [HttpPost]
        public string SendMail(string subject, string body, string to)
        {
            try
            {
                var SMTPServer = ConfigurationSettings.SMTPServer;
                var Mail = ConfigurationSettings.Mail;
                var Password = ConfigurationSettings.Password;
                var Port = ConfigurationSettings.Port;

                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(SMTPServer);

                mail.From = new MailAddress(Mail);
                mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = body;

                SmtpServer.Port = Port;
                SmtpServer.Credentials = new System.Net.NetworkCredential(Mail, Password);
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);

                return "Ok";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}