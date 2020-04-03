using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Models;
using Microsoft.Extensions.Logging;
using EmployeeManagement.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeManagement.Controllers
{
    public class OfficeController : Controller
    {
        private IEmployeeRepository _employeeRepository;
        private IOfficeRepository _officeRepository;
        private ILogger logger;

        public OfficeController(IEmployeeRepository employeeRepository, IOfficeRepository officeRepository, ILogger<OfficeController> logger)
        {
            _employeeRepository = employeeRepository;
            _officeRepository = officeRepository;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = _officeRepository.GetAllOffices();
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(OfficeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                Office newOffice = new Office
                {
                    Name = model.Name,
                    MinimumManning = model.MinimumManning
                };

                _officeRepository.Add(newOffice);
                return RedirectToAction("index", new { id = newOffice.Id });
            }
            return View();
        }

        [HttpGet]
        public IActionResult Details(string id)
        {
            Office office = _officeRepository.GetOffice(id);

            if(office == null)
            {
                Response.StatusCode = 404;
                return View("OfficeNotFound", id);
            }

            var currentManning = office.Employees.Count();

            OfficeDetailsViewModel officeDetailsViewModel = new OfficeDetailsViewModel()
            {
                Office = office,
                CurrentManning = currentManning
            };

            return View(officeDetailsViewModel);
        }
    }
}
