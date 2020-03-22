using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.ViewModels
{
    public class UserClaim
    {
        public string ClaimType { get; set; }
        // Determines if the claim is selected on the UI
        public bool IsSelected { get; set; }
    }
}
