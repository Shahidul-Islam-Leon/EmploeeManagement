
using EmployeeManagement.Models;
using EmployeeManagement.Repository;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private readonly IEmployeeRepository _employeeRepository;

        private readonly IWebHostEnvironment _hostingEnvironment;

        public HomeController(IEmployeeRepository employeeRepository, IWebHostEnvironment hostingEnvironment)
        {
            _employeeRepository = employeeRepository;
            _hostingEnvironment = hostingEnvironment;
        }


        [HttpGet]
        [AllowAnonymous]
        public ViewResult Index()
        {
            var allEmployee = _employeeRepository.GetAllEmployee();
            return View(allEmployee);
        }

        [HttpGet]
        public ViewResult Details(int id)
        {
            var model = _employeeRepository.GetEmployee(id);

            return View(model);
        }


        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]

        public IActionResult Create(CreateEmployeeViewModel employee)

        {


            if (ModelState.IsValid)
            {
                string uniqueFileName = PhotoProcess(employee);

                Employee newEmployee = new()
                {
                    Name = employee.Name,
                    Email = employee.Email,
                    Department = employee.Department,
                    PhotoPath = uniqueFileName
                };

                _employeeRepository.Create(newEmployee);
                return RedirectToAction("details", new { id = newEmployee.Id });
            }

            return View();
        }

        private string PhotoProcess(CreateEmployeeViewModel employee)
        {
            string uniqueFileName = null;

            if (employee.Photo != null)
            {

                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + employee.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                employee.Photo.CopyTo(new FileStream(filePath, FileMode.Create));

            }
            return uniqueFileName;

        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var employee = _employeeRepository.GetEmployee(id);




            EditEmployeeViewModel editEmployeeViewModel = new()
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };

            return View(editEmployeeViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EditEmployeeViewModel editEmployeeViewModel)
        {
            if (ModelState.IsValid)
            {
                var getEmployee = _employeeRepository.GetEmployee(editEmployeeViewModel.Id);

                getEmployee.Name = editEmployeeViewModel.Name;
                getEmployee.Email = editEmployeeViewModel.Email;
                getEmployee.Department = editEmployeeViewModel.Department;


                if (editEmployeeViewModel.Photo != null)
                {

                    if (editEmployeeViewModel.ExistingPhotoPath != null)
                    {
                        string filepath = Path.Combine(_hostingEnvironment.WebRootPath,
                            "images", editEmployeeViewModel.ExistingPhotoPath);
                        System.IO.File.Delete(filepath);
                    }

                    getEmployee.PhotoPath = PhotoProcess(editEmployeeViewModel);

                }

                _employeeRepository.Update(getEmployee);
                return RedirectToAction("index");
            }
            return View();
        }
    }

}

