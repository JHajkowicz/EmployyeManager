using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmployyeManager.Models
{
    public class UserInputModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6)]
        public string Password { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [MaxLength(255)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required.")]
        [MaxLength(255)]
        public string LastName { get; set; }
    }
}