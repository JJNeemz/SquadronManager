using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace EmployeeManagement.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IWebHostEnvironment hostingEnvironrment;
        private readonly ILogger logger;
        private readonly IOfficeRepository _officeRepository;
        private readonly AppDbContext _context;

        //Injecting this dependency means that we do not have to manually change all controllers
        //that use this dependency if we want to change how we get our employees. All we need to do
        //is change the Startup.cs file and all controllers using the IEmployeeRepository injection
        //will inject the correct model and reflect the changes.
        public EmployeeController(IEmployeeRepository employeeRepository, IWebHostEnvironment hostingEnvironrment, 
            ILogger<EmployeeController> logger, IOfficeRepository officeRepository, AppDbContext context)
        {

            _employeeRepository = employeeRepository;
            _officeRepository = officeRepository;
            _context = context;
            this.hostingEnvironrment = hostingEnvironrment;
            this.logger = logger;
        }

        private string ProcessUploadedFile(EmployeeCreateViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photo != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironrment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                //Utilize Using statement to properly dispose of filestream
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                };

            }

            return uniqueFileName;
        }

        private static string GetEnumDisplayName(Afsc? name)
        {
            if (name != null)
            {
                return name.GetType().GetMember(name.ToString())
                           .First().GetCustomAttribute<DisplayAttribute>().Name;
            }
            else
            {
                return "No Registered AFSC";
            }



        }


        [AllowAnonymous]
        public ViewResult Index(string sortType)
        {
            sortType = String.IsNullOrEmpty(sortType) ? "lastName_asc" : sortType;

            var model = _employeeRepository.GetAllEmployee();
            // Use switch statement so we can easily add or remove sort criteria
            switch (sortType)
            {
                case "lastName_asc":
                    model = model.OrderBy(e => e.LastName).ToList();
                    break;
                case "lastName_desc":
                    model = model.OrderByDescending(e => e.LastName).ToList();
                    break;
            }

            return View(model);
        }

        [HttpGet]
        public ViewResult Create()
        {
            // TODO : Refactor logic into EmployeeCreateViewModel
            var listOfOffices = _officeRepository.GetAllOffices();
            List<SelectListItem> officeList = new List<SelectListItem>();
            foreach (Office office in listOfOffices)
            {
                officeList.Add(new SelectListItem
                {
                    Value = office.Id,
                    Text = office.Name
                });
            }

            EmployeeCreateViewModel model = new EmployeeCreateViewModel()
            {
                OfficeList = officeList
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFile(model);
                //var office = _officeRepository.GetOffice(model.OfficeId);

                Employee newEmployee = new Employee
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    OfficeId = model.OfficeId,
                    PhotoPath = uniqueFileName,
                    Afsc = model.Afsc,
                    Rank = model.Rank
                };

                _employeeRepository.Add(newEmployee);

                return RedirectToAction("details", new { id = newEmployee.Id });
            }
            else
            {
                return View();
            }
        }


        [AllowAnonymous]
        public ViewResult Details(int? id)
        {

            Employee employee = _employeeRepository.GetEmployee(id.Value);

            // Handle 404 resource error
            if (employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id.Value);
            }

            string officeName = "Not Assigned To An Office";
            if(employee.OfficeId != null)
            {
                officeName = employee.Office.Name;
            }

            EmployeeDetailsViewModel employeeDetailsViewModel = new EmployeeDetailsViewModel()
            {
                Employee = employee,
                Afsc = employee.Afsc,
                AfscDisplayName = GetEnumDisplayName(employee.Afsc),
                PageTitle = "Employee Details",
                OfficeName = officeName,
                Rank = employee.Rank
            };
            return View(employeeDetailsViewModel);
        }


        [HttpGet]
        public ViewResult Edit(int id)
        {
            Employee employee = _employeeRepository.GetEmployee(id);

            // TODO : Refactor logic into EmployeeCreateViewModel
            var listOfOffices = _officeRepository.GetAllOffices();
            List<SelectListItem> officeList = new List<SelectListItem>();
            foreach (Office office in listOfOffices)
            {
                officeList.Add(new SelectListItem
                {
                    Value = office.Id,
                    Text = office.Name
                });
            }

            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel
            {
                Id = id,
                Employee = employee,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                OfficeId = employee.OfficeId,
                OfficeList = officeList,
                Afsc = employee.Afsc,
                Rank = employee.Rank,
                ExistingPhotoPath = employee.PhotoPath

            };
            return View(employeeEditViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = _employeeRepository.GetEmployee(model.Id);
                employee.FirstName = model.FirstName;
                employee.LastName = model.LastName;
                employee.Email = model.Email;
                employee.OfficeId = model.OfficeId;
                employee.Afsc = model.Afsc;
                employee.Rank = model.Rank;
                if (model.Photo != null)
                {
                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(hostingEnvironrment.WebRootPath, "images", model.ExistingPhotoPath);
                        // Use system.IO File class to delete the existing photo
                        System.IO.File.Delete(filePath);
                    }
                    // Saves the uploaded file to the images folder and returns us the unique file name
                    employee.PhotoPath = ProcessUploadedFile(model);
                }

                // Entity framework Update method updates the respective employee record in the underlying database
                _employeeRepository.Update(employee);
                return RedirectToAction("index");
            }
            else
            {
                return View();
            }
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            // Delete employee from repository
            _employeeRepository.Delete(id);
            // Get updated list of employees to pass to the index to list
            var result = _employeeRepository.GetAllEmployee();
            return View("index", result);
        }

    }
}
