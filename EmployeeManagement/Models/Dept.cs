using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public enum Dept
    {
        None,

        Infrastructure,

        [Display(Name = "Network Operations")]
        NetworkOperations,

        [Display(Name = "Command Support")]
        CommandSupport,

        [Display(Name = "Information Assurance")]
        InformationAssurance,

        [Display(Name = "Client Systems")]
        ClientSystems,

        [Display(Name = "Help Desk")]
        HelpDesk,

        [Display(Name = "Project Management")]
        ProjectManagement,

        [Display(Name = "Enlisted Leadership")]
        EnlistedLeadership,

        [Display(Name = "Officer Leadership")]
        OfficerLeadership
    }
}
