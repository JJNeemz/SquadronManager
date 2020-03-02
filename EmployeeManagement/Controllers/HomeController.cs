using System;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;

        //Injecting this dependency means that we do not have to manually change all controllers
        //that use this dependency if we want to change how we get our employees. All we need to do
        //is change the Startup.cs file and all controllers using the IEmployeeRepository injection
        //will inject the correct model and reflect the changes.
        public HomeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public string index()
        {
            return _employeeRepository.GetEmployee(1).Name;
        }

        public ViewResult details()
        {
            Employee model = _employeeRepository.GetEmployee(1);
            return View(model);
        }
    }
}
