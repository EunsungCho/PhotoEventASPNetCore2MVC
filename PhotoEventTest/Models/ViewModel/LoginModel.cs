using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PhotoEventTest.Models;
using System.ComponentModel.DataAnnotations;

namespace PhotoEventTest.Models.ViewModel
{
    public class LoginModel
    {   
        [Required]
        public string UserId { get; set; }

        [Required]
        [UIHint("password")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}
