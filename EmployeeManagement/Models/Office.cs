using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class Office
    {
        public Office()
        {
            Employees = new List<Employee>();
        }   

        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int MinimumManning { get; set; }
        public int CurrentManning { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}
