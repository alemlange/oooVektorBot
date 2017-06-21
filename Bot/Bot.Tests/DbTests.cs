using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LiteDbService;
using DataModels;
using System.Collections.Generic;

namespace Bot.Tests
{
    [TestClass]
    public class DbTests
    {
        [TestMethod]
        public void TestMenuCreation()
        {
            var service = new LiteManagerService();
            var menuId = service.CreateNewMenu(new Menu { DishList = new List<Dish> { new Dish { Id = Guid.NewGuid(), Name = "Кашка"} } });
            var menu = service.GetMenu(menuId);
        }

        [TestMethod]
        public void TestGetAllMenus()
        {
            var service = new LiteManagerService();
            var menu = service.GetAllMenus();
        }
    }
}
