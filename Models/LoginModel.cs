using System;
using System.ComponentModel.DataAnnotations;

using asp_net.Authentication;
namespace asp_net.Models
{
	public class LoginModel
	{
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}

