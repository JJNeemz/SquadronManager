using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class SQLOfficeRepository : IOfficeRepository
    {
        private readonly AppDbContext context;

        public SQLOfficeRepository(AppDbContext context)
        {
            this.context = context;
        }

        public Office Add(Office office)
        {
            context.Offices.Add(office);
            context.SaveChanges();
            return office;
        }

        public Office Delete(int id)
        {
            Office office = context.Offices.Find(id);
            if(office != null)
            {
                context.Offices.Remove(office);
                context.SaveChanges();
            }
            
            return office;
        }

        public IEnumerable<Office> GetAllOffices()
        {
            return context.Offices;
        }

        public Office GetOffice(int id)
        {
            return context.Offices.Find(id);
        }

        public Office Update(Office officeChanges)
        {
            var office = context.Offices.Attach(officeChanges);
            office.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return officeChanges;
        }
    }
}
