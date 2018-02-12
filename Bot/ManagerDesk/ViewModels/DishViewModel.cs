using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataModels;
using System.ComponentModel.DataAnnotations;

namespace ManagerDesk.ViewModels
{
    public class DishViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Нужно ввести название блюда")]
        public string Name { get; set; }

        public string ShortName { get; set; }

        public string PictureUrl { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public List<int> ModIds { get; set; }

        [Required(ErrorMessage = "Нужно ввести цену блюда")]
        [Range(1, int.MaxValue, ErrorMessage = "Цена должна быть больше 0")]
        public decimal Price { get; set; }

        public List<string> AvailableCategories { get; set; }

        public string PriceString
        {
            get
            {
                return Price == 0 ? "" : Price.ToString();
            }
        }

        public string InstImageUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(PictureUrl))
                {
                    return PictureUrl + "/media";
                }
                else
                    return "/Assets/Imgs/insta_placeholder.png";
            }
        }
    }
}