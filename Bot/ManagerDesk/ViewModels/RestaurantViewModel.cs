using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ManagerDesk.ViewModels
{
    public class RestaurantViewModel 
    {
        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Не заполнено название ресторана")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Нужно ввести адрес ресторана")]
        public string Address { get; set; }

        public string Description { get; set; }

        public string Code { get; set; }

        public int QueueNumber { get; set; }

        public Guid Menu { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

    }
}