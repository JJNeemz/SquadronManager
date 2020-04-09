using EmployeeManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.ViewModels
{
    public class EmployeeCreateViewModel
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string LastName { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$",
            ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        public IFormFile Photo { get; set; }
        [Display(Name="Office")]
        public string OfficeId { get; set; }

        public List<Office> Offices { get; set; }

        // Create SelectListItem list to use for dropdown
        public List<SelectListItem> OfficeList { get; set; }
        [Display(Name="Air Force Specialty Code")]
        public Afsc? Afsc { get; set; }
        public Rank Rank { get; set; }
    }
}
