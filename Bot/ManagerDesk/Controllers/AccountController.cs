using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ManagerDesk.Models;
using ManagerDesk.Services;
using AutoMapper;
using DataModels;
using DataModels.Configuration;

namespace ManagerDesk.Controllers
{

    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {

                var regService = new RegistrationService();
                
                 if (regService.Login(model.Name, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.Name, true);
                    return RedirectToAction("Index", "Manager");
                }
                else
                {
                    ModelState.AddModelError("", "Пользователь с таким логином и паролем не существует.");
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult TestAccount()
        {
            var regService = new RegistrationService();

            if (regService.Login(ConfigurationSettings.TestLogin, ConfigurationSettings.TestPassword))
            {
                FormsAuthentication.SetAuthCookie(ConfigurationSettings.TestLogin, true);
                return RedirectToAction("Index", "Manager");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {

                if (Validate(model))
                {
                    var regService = new RegistrationService();
                    regService.Register(model.Name, model.Password);

                    FormsAuthentication.SetAuthCookie(model.Name, true);
                    return RedirectToAction("Index", "Manager");
                    
                }
                else
                {
                    if(model.ConfirmPassword != model.Password)
                        ModelState.AddModelError("", "Пароль и подтверждение пароля не совпадают");
                    else
                        ModelState.AddModelError("", "Пользователь с таким логином уже существует");
                }
            }

            return View(model);
        }

        private bool Validate(RegisterModel model)
        {
            try
            {
                return !(model.ConfirmPassword != model.Password || new RegistrationService().FindAccount(model.Name) != null);
            }
            catch 
            {
                return false;
            }
        }

        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Manager");
        }
    }
    
}