using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public enum Rank
    {
        [Display(Name ="Airman Basic")]
        AirmanBasic,
        Airman,
        A1C,
        SrA,
        SSgt,
        TSgt,
        MSgt,
        SMSgt,
        CMSgt
    }
}
