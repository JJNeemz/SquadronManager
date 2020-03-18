﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        //Create constructor for DbContext options used for configuration information 
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {

        }

        //This property is used to query and saves instances of the Employee class
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Keys of identity tables are mapped on OnModelCreating method of IdentityDbContext class
            base.OnModelCreating(modelBuilder);
            modelBuilder.Seed();
        }
    }
}
