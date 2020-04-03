using EmployeeManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.ViewModels
{
    public class OfficeIndexViewModel
    {
        IEnumerable<Office> Offices { get; set; }
        public string OfficeId { get; set; }
        public string OfficeName { get; set; }
        public int MinimumManning { get; set; }
        public int CurrentManning { get; set; }
    }
}
