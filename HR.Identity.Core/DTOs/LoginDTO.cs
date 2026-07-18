using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Identity.Core.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "وارد کردن نام کاربری الزامی می باشد")]
        [MinLength(3)]
        public string UserName { get; set; }
        [Required(ErrorMessage = "وارد کردن کلمه عبور الزامی می باشد")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        //[Required(ErrorMessage = "وارد کردن تکرار کلمه عبور الزامی می باشد")]
        //[StringLength(255, ErrorMessage = "طول کلمه عبور بایدد حداقل 5 کاراکتر باشد", MinimumLength = 5)]
        //[DataType(DataType.Password)]
        //[Compare("Password", ErrorMessage = "تکرار کلمه عبور مطابقت ندارد ")]
        //public string ConfirmPassword { get; set; }
        ///public bool RememberMe { get; set; }
    }
}
