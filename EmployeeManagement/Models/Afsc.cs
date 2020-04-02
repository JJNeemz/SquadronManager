using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public enum Afsc
    {
        [Display(Name ="3D0X1")]
        KnowledgeOperationsManagement,
        [Display(Name = "3D0X2")]
        CyberSystemsOperations,
        [Display(Name = "3D0X3")]
        CyberSurety,
        [Display(Name = "3D0X4")]
        ComputerSystemsProgramming,
        [Display(Name = "3D1X1")]
        ClientSystems,
        [Display(Name = "3D1X2")]
        CyberTransportSystems,
        [Display(Name = "3D1X3")]
        RFTransmissionSystems,
        [Display(Name = "3D1X7")]
        CableAndAntennaSystems
    }
}
