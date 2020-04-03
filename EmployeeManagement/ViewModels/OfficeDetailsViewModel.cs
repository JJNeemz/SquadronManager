using EmployeeManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.ViewModels
{
    public class OfficeDetailsViewModel
    {
        public Office Office { get; set; }
        public int CurrentManning { get; set; }
    }
}
