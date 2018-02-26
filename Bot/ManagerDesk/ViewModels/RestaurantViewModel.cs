using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.ObjectModel;
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

        public string TimeZoneId { get; set; }

        public float Latitude { get; set; }

        public float Longitude { get; set; }

    }
}