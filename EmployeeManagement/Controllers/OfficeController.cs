using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Models;
using Microsoft.Extensions.Logging;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeManagement.Controllers
{
    [Authorize]
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

        [HttpGet]
        public IActionResult Edit(string id)
        {
            Office office = _officeRepository.GetOffice(id);
            OfficeEditViewModel model = new OfficeEditViewModel()
            {
                Office = office,
                Id = id
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(OfficeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Office office = _officeRepository.GetOffice(model.Id);
                office.Name = model.Office.Name;
                office.MinimumManning = model.Office.MinimumManning;

                _officeRepository.Update(office);
                return RedirectToAction("index");
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult ManageEmployees(string officeId)
        {
            var office = _officeRepository.GetOffice(officeId);
            var employees = _employeeRepository.GetAllEmployee();
            // Initialize OfficeManageEmployeesViewModel to hold all of the different employee information
            var model = new List<OfficeManageEmployeesViewModel>();

            if (office != null)
            {
                ViewBag.officeId = officeId;
                ViewBag.officeName = office.Name;
                

                foreach(Employee employee in employees)
                {

                    OfficeManageEmployeesViewModel officeManageEmployeesViewModel = new OfficeManageEmployeesViewModel()
                    {
                        EmployeeId = employee.Id,
                        EmployeeName = employee.Name,
                        EmployeeOfficeId = employee.OfficeId,
                        CurrentOfficeId = officeId
                    };

                    //Check if employee belongs to this office and assign to bool
                    if (employee.OfficeId == officeId)
                    {
                        officeManageEmployeesViewModel.IsSelected = true;
                    } 
                    else
                    {
                        officeManageEmployeesViewModel.IsSelected = false;
                    }
                    model.Add(officeManageEmployeesViewModel);
                }
                return View(model);

            }

            ViewBag.ErrorMessage = $"The Office ID {officeId} is invalid";
            return View("NotFound");
        }

        [HttpPost]
        public IActionResult ManageEmployees(List<OfficeManageEmployeesViewModel> model)
        {
            for(var i = 0; i < model.Count; i++)
            {
                if(model[i].IsSelected && model[i].EmployeeOfficeId != model[i].CurrentOfficeId)
                {
                    var employee = _employeeRepository.GetEmployee(model[i].EmployeeId);
                    employee.OfficeId = model[i].CurrentOfficeId;
                    _employeeRepository.Update(employee);
                }
                else
                {
                    continue;
                }
            }
            return RedirectToAction("index");
        }
    }
}
