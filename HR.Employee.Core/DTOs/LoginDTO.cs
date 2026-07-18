using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "وارد کردن نام کاربری الزامی می باشد")]
        [MinLength(6)]
        public string UserName { get; set; }
        [Required(ErrorMessage = "وارد کردن کلمه عبور الزامی می باشد")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
