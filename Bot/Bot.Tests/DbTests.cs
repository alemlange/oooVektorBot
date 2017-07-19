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
                    MenuName = "Меню Fridays",
                    DishList = new List<Dish> {
                        new Dish { Id = Guid.NewGuid(), Category="Напитки" ,Name = "Кувшин Сплэш Мятный Апельсин", Description = "Свежая мята, апельсин, апельсиновый сок, сауэр микс, сахарный сироп, содовая.", Price = 625},
                        new Dish { Id = Guid.NewGuid(), Category="Напитки" ,Name = "Кувшин Домашнего Лимонада FRIDAYS", Description = "Лимон, сок лимона, сауэр микс, сахарный сироп, содовая", Price = 625},
                        new Dish { Id = Guid.NewGuid(), Category="Напитки" ,Name = "Кувшин Вишнево-Лаймовый Краш", Description = "Гранатовый сироп, сауэр микс, сахарный сироп, Спрайт®, вишня, лайм", Price = 625},
                        new Dish { Id = Guid.NewGuid(), Category="Напитки" ,Name = "Кувшин Сплэш Имбирное Лето", Description = "Имбирный напиток, сауэр микс, ванильный сироп, содовая и лайм. ", Price = 625},
                        new Dish { Id = Guid.NewGuid(), Category="Напитки" ,Name = "Кувшин Сплэш Маракуйя Апельсин", Description = "Апельсин, пюре и сироп маракуйи, апельсиновый сок, сауэр микс, содовая, Cпрайт.", Price = 625},

                        new Dish { Id = Guid.NewGuid(), Category="Закуски" ,Name = "КАРТОФЕЛЬНЫЕ ДОЛЬКИ С СЫРОМ И ПЕППЕРОНИ", Description = "Картофельные дольки с соусом Кеса Чиз и пепперони.", Price = 199},
                        new Dish { Id = Guid.NewGuid(), Category="Закуски" ,Name = "ХЛЕБНЫЕ ШАРИКИ С СЫРОМ", Description = "Идеальная закуска к кружечке холодненького. Воздушные хлебные шарики с сыром, хрустящие снаружи и мягкие внутри. Подаются с соусом Кеса Чиз", Price = 299},
                        new Dish { Id = Guid.NewGuid(), Category="Закуски" ,Name = "КУРИНАЯ СТРУЖКА ДЖЕК ДЕНИЕЛС", Description = "Аппетитно обжаренные тонкие кусочки куриного филе в панировке с кунжутом, подаются с соусом Джек Дениелс", Price = 445},
                        new Dish { Id = Guid.NewGuid(), Category="Закуски" ,Name = "ЖАРЕНЫЙ СЫР", Description = "Сыр, обжаренный в панировке с итальянскими приправами. Подается с соусом Маринара. .", Price = 425},
                        new Dish { Id = Guid.NewGuid(), Category="Закуски" ,Name = "КЕСАДИЙЯ С КУРИЦЕЙ", Description = "Хрустящая пшеничная лепешка, фаршированная кусочками куриного филе и белым сыром, украшенная соусом Бальзамик. Подается с соусом Маринара.", Price = 489},

                        new Dish { Id = Guid.NewGuid(), Category="Бургеры" ,Name = "ИТАЛЬЯНСКИЙ БУРГЕР", Description = "Наш фирменный бургер с хрустящим жареным сыром, колбасками пепперони, базиликом, брушетой маринара, сыром и соусом хорсредиш. Подается с соусами маринара и хорсредиш.", Price = 675},
                        new Dish { Id = Guid.NewGuid(), Category="Бургеры" ,Name = "БУРГЕР БЛЮ ЧИЗ С БЕКОНОМ", Description = "Заряжен Двойной Дозой вкуса – ароматным соусом Блю Чиз, сыром и беконом. ", Price = 599},
                        new Dish { Id = Guid.NewGuid(), Category="Бургеры" ,Name = "БУРГЕР ДЖЕК ДЕНИЕЛС®", Description = "Сочная говядина в нашем фирменном соусе Джек Дениелс®, с хрустящими полосками бекона и белым сыром. Подается с соусом Джек Дениелс®", Price = 565},

                        new Dish { Id = Guid.NewGuid(), Category="Стейки" ,Name = "НЬЮ-ЙОРК СТРИП СТЕЙК", Description = "", Price = 1100},
                        new Dish { Id = Guid.NewGuid(), Category="Стейки" ,Name = "ФИЛЕ МИНЬОН", Description = "ВРЕМЯ ПРИГОТОВЛЕНИЯ БЛЮДА ОТ 25 МИНУТ", Price = 799},
                        new Dish { Id = Guid.NewGuid(), Category="Стейки" ,Name = "ТЕНДЕРЛОЙН СТЕЙК", Description = "", Price = 699},

                        new Dish { Id = Guid.NewGuid(), Category="Фирменные блюда" ,Name = "ЭКСТРИМ БАФФАЛО ТАКО C КУРИЦЕЙ", Description = "Кукурузные лепешки тортийя, фаршированные кусочками куриной грудки в пикантном соусе Баффало на основе соуса TABASCO® оригинальный красный перечный, с соусом Блю чиз и салатом. Посыпаются Пико де Гайо и подаются с картофелем фри", Price = 490},
                        new Dish { Id = Guid.NewGuid(), Category="Фирменные блюда" ,Name = "ЮГО-ЗАПАДНАЯ КЕСАДИЙЯ С КУРИЦЕЙ", Description = "Пшеничная Лепешка тортийя, обжаренная на гриле и фаршированная курицей, сыром, карамелизированным луком, пепперони, помидорами. Подается со сметаной, Пико де Гайо и гуакамоле.", Price = 535},
                        new Dish { Id = Guid.NewGuid(), Category="Фирменные блюда" ,Name = "КРЕВЕТКИ FRIDAYS", Description = "Креветки, панированные и обжаренные до хрустящей корочки. Подаются с соусом Тар-Тар и картофелем фри", Price = 715},
                        new Dish { Id = Guid.NewGuid(), Category="Фирменные блюда" ,Name = "БЕФСТРОГАНОВ НЬЮ-ЙОРК СТАЙЛ", Description = "Нежнейшая говядина обжаренная с луком, шампиньонами и соусом деми глас. Подается с картофельным пюре с сыром и глазированной морковью Джек Дениелс®", Price = 445},
                        new Dish { Id = Guid.NewGuid(), Category="Фирменные блюда" ,Name = "КУРИЦА МАЙАМИ КУБАНО", Description = "Две нежные куриные грудки в нашей фирменной панировке с сыром, ветчиной, грибами, сливочным соусом, Пико де Гайо и помидорами. Подаются с пряным рисом. ", Price = 599},
                        new Dish { Id = Guid.NewGuid(), Category="Фирменные блюда" ,Name = "КУРИНЫЕ ПАЛЬЧИКИ", Description = "Обжаренные до золотистого цвета полоски куриного филе в панировке. Подаются украшенные медом, с картофелем фри и медово-горчичной заправкой. По вашему вкусу мы можем добавить специи Кейджин", Price = 465}}
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
            var tableId = service.CreateTable(101);
            Assert.AreNotEqual(Guid.Empty, tableId);
        }
    }
}
