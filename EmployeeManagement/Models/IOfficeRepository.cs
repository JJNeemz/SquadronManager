using System;
using System.Collections;
using System.Collections.Generic;

namespace EmployeeManagement.Models
{
    public interface IOfficeRepository
    {
        Office GetOffice(string id);
        IEnumerable<Office> GetAllOffices();
        Office Add(Office office);
        Office Update(Office officeChanges);
        Office Delete(string id);
    }
}
