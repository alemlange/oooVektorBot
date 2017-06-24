using System;
using System.Linq;
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
            var service = new TestLiteManagerService();
            var menuId = service.CreateNewMenu(new Menu { DishList = new List<Dish> { new Dish { Id = Guid.NewGuid(), Name = "Кашка"} } });
            var menu = service.GetMenu(menuId);
        }

        [TestMethod]
        public void TestGetAllMenus()
        {
            var service = new TestLiteManagerService();
            var menu = service.GetAllMenus();
        }

        [TestMethod]
        public void OrderDish()
        {
            var service = new TestLiteCustomerService();
            var dishToOrder = service.GetAllMenus().First().DishList.First();
            var orderedDish = new OrderedDish() { Remarks = "Побольше перца", DishFromMenu = dishToOrder };

            service.OrderDish(Guid.Parse("c7e12b4a-289b-4cd5-b71f-0379d4755a42"), orderedDish);
        }
        [TestMethod]
        public void GetAllTables()
        {
            var service = new TestLiteManagerService();
            var tables = service.GetAllTables();
        }
        [TestMethod]
        public void CreateTable()
        {
            var service = new TestLiteCustomerService();
            var tableId = service.CreateTable();
            Assert.AreNotEqual(Guid.Empty, tableId);
        }
    }
}
