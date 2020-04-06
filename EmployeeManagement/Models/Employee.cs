using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string FirstName { get; set;}

        [Required]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string LastName { get; set; }

        [Required]
        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
            ErrorMessage ="Invalid email format")]
        [Display(Name = "Office Email")]
        public string Email { get; set; }

        public string PhotoPath { get; set; }
        public string OfficeId { get; set; }
        public Office Office { get; set; }
        public Rank? Rank { get; set; }
        public Afsc? Afsc { get; set; }
    }
}
