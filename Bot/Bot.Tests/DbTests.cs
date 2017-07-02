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
            var menuId = service.CreateNewMenu(
                new Menu {
                    MenuName = "Меню ресторана Бургерная №1",
                    DishList = new List<Dish> {
                        new Dish { Id = Guid.NewGuid(), Name = "Бургер", Description = "с сыром и каперсами", Price = 350},
                        new Dish { Id = Guid.NewGuid(), Name = "Картошка", Description = "", Price = 100},
                        new Dish { Id = Guid.NewGuid(), Name = "Пицца", Description = "готовится в дровяной печи", Price = 400},
                        new Dish { Id = Guid.NewGuid(), Name = "Ребрышки", Description = "вкуснейшие говяжьи ребра ", Price = 500}}
                });

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
            var serviceManager = new TestLiteManagerService();
            var rand = new Random();
            
            var dishList = service.GetAllMenus().First().DishList;
            var randomDish = dishList[rand.Next(dishList.Count)];

            var orderedDish = new OrderedDish() { Remarks = "Побольше перца", DishFromMenu = randomDish };

            var tableList = serviceManager.GetAllTables();
            var randomTable = tableList[rand.Next(tableList.Count)];

            service.OrderDish(randomTable.Id, orderedDish);
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
            var tableId = service.CreateTable(101,101);
            Assert.AreNotEqual(Guid.Empty, tableId);
        }
    }
}
