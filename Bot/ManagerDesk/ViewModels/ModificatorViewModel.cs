using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataModels;
using System.ComponentModel.DataAnnotations;

namespace ManagerDesk.ViewModels
{
    public class ModificatorViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Нужно ввести название блюда")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Нужно ввести макисимальное количество добавлений")]
        [Range(1, 100, ErrorMessage = "Количество должно быть больше 0")]
        public int MaxCount { get; set; }

        public decimal Price { get; set; }

        public string PriceString
        {
            get
            {
                return Price == 0 ? "" : Price.ToString();
            }
        }

        public string MaxCountString
        {
            get
            {
                return Price == 0 ? "" : Price.ToString();
            }
        }
    }
}