using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerDesk.Models;
using AutoMapper;
using System.Security.Cryptography;
using DataModels;
using LiteDbService.Helpers;

namespace ManagerDesk.Services
{
    public class RegistrationService
    {
        protected SHA512 Provider = new SHA512CryptoServiceProvider();

        private string Encrypt(string s)
        {
            var bytes = Encoding.Default.GetBytes(s);
            var encryptedBytes = Provider.ComputeHash(bytes);
            var hash = BitConverter.ToString(encryptedBytes);
            return hash.Where(t => t != '-').Aggregate("", (current, t) => current + t);
        }

        public void Register(string login, string password)
        {
            var hash = Encrypt(password);
            var account = new ManagerAccount { Id = Guid.NewGuid(), PasswordHash = hash, Login = login };

            var service = ServiceCreator.GetRegistrationService();
            service.CreateAccount(account);

        }

        public bool Login(string login, string password)
        {
            var hash = Encrypt(password);

            var service = ServiceCreator.GetRegistrationService();
            var account = service.FindAccount(login);

            return account != null && account.PasswordHash == hash;
        }

        public ManagerAccount FindAccount(string login)
        {
            var service = ServiceCreator.GetRegistrationService();
            return service.FindAccount(login);

        }
    }
}