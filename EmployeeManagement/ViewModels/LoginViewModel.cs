using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

        // Used to preserve URL that user is at when they are prompted to login
        // so that we can return them to the page they were trying to access after authenticating
        public string ReturnUrl { get; set; }

        public int MyProperty { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
    }
}
