using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ManagerDesk.Models
{
    public class LoginModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage ="Не заполнено поле логина")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Не заполнено поле пароля")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}