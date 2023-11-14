using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace asp_net.Models
{

    [Keyless]
    public class GoogleLoginModel
    {
        [Required(ErrorMessage = "Google ID token is required")]
        public string IdToken { get; set; }

      
    }
}
