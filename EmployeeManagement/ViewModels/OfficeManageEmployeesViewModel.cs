﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.ViewModels
{
    public class OfficeManageEmployeesViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public bool IsSelected { get; set; }
        public string EmployeeOfficeId { get; set; }
        public string CurrentOfficeId { get; set; }
    }
}
