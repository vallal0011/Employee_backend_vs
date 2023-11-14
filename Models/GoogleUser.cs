using System;
using System.ComponentModel.DataAnnotations;

namespace asp_net.Models
{
    public class GoogleUser
    {
        [Key] // Add this attribute to define the 'Id' property as the primary key
        public string Id { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        // Add more properties as needed to capture user information
    }
}

