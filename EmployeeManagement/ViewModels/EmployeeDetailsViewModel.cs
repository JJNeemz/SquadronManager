using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.ViewModels
{
    public class EmployeeDetailsViewModel
    {
        public Employee Employee { get; set; }
        public string PageTitle { get; set; }
        public string OfficeId { get; set; }
        public string OfficeName { get; set; }
        public Afsc? Afsc { get; set; }
        public string AfscDisplayName { get; set; }

        public List<Office> Offices { get; set; }

        // Create SelectListItem list to use for dropdown
        public List<SelectListItem> OfficeList { get; set; }
        public Rank Rank { get; set; }
    }
}
