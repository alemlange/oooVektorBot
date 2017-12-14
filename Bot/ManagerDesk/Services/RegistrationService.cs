using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerDesk.Models;
using AutoMapper;
using System.Security.Cryptography;
using DataModels;
using DataModels.Exceptions;
using LiteDbService.Helpers;

namespace ManagerDesk.Services
{
    public class RegistrationService
    {
        //protected SHA512 Provider = new SHA512CryptoServiceProvider();

        private string Encrypt(string s)
        {
            var provider = new SHA512CryptoServiceProvider();
            var bytes = Encoding.Default.GetBytes(s);
            var encryptedBytes = provider.ComputeHash(bytes);
            var hash = BitConverter.ToString(encryptedBytes);
            return hash.Where(t => t != '-').Aggregate("", (current, t) => current + t);
        }

        public void Register(string login, string password)
        {
            var hash = Encrypt(password);
            var account = new ManagerAccount { Id = Guid.NewGuid(), PasswordHash = hash, Login = login };

            var service = ServiceCreator.GetRegistrationService();
            service.CreateAccount(account);
            service.CreateConfig(account.Id);

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

        public Config FindConfiguration(string login)
        {
            var service = ServiceCreator.GetRegistrationService();
            var acc =  service.FindAccount(login);

            if (acc != null)
                return service.FindConfig(acc.Id);
            else
                throw new AuthException("Account not found");

        }

        public void UpdateConfiguration(Config config)
        {
            var service = ServiceCreator.GetRegistrationService();
            service.UpdateConfig(config);

        }
    }
}