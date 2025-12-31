using Microsoft.AspNetCore.Mvc;
using MOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOM.Controllers
{
    public class DepartmentController : Controller
    {
        // Static list to simulate database
        private static List<Department> _departments = new List<Department>
        {
            new Department { Id = 1, Name = "Human Resources", Code = "HR", Description = "Manages employee relations and recruitment", Manager = "John Smith", EmployeeCount = 15, CreatedDate = new DateTime(2020, 1, 15), IsActive = true },
            new Department { Id = 2, Name = "Information Technology", Code = "IT", Description = "Handles all technology infrastructure", Manager = "Sarah Johnson", EmployeeCount = 25, CreatedDate = new DateTime(2019, 6, 10), IsActive = true },
            new Department { Id = 3, Name = "Finance", Code = "FIN", Description = "Manages financial operations and accounting", Manager = "Michael Brown", EmployeeCount = 12, CreatedDate = new DateTime(2018, 3, 22), IsActive = true },
            new Department { Id = 4, Name = "Marketing", Code = "MKT", Description = "Marketing and brand management", Manager = "Emily Davis", EmployeeCount = 18, CreatedDate = new DateTime(2021, 8, 5), IsActive = true },
            new Department { Id = 5, Name = "Operations", Code = "OPS", Description = "Daily operations and logistics", Manager = "David Wilson", EmployeeCount = 30, CreatedDate = new DateTime(2017, 11, 18), IsActive = true },
            new Department { Id = 6, Name = "Sales", Code = "SLS", Description = "Sales and customer relations", Manager = "Jessica Martinez", EmployeeCount = 22, CreatedDate = new DateTime(2019, 2, 14), IsActive = true },
            new Department { Id = 7, Name = "Research & Development", Code = "R&D", Description = "Product research and development", Manager = "Robert Taylor", EmployeeCount = 20, CreatedDate = new DateTime(2020, 9, 30), IsActive = true },
            new Department { Id = 8, Name = "Customer Service", Code = "CS", Description = "Customer support and service", Manager = "Lisa Anderson", EmployeeCount = 28, CreatedDate = new DateTime(2018, 7, 12), IsActive = false }
        };

        public IActionResult Index()
        {
            return View(_departments);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Department department)
        {
            if (ModelState.IsValid)
            {
                department.Id = _departments.Any() ? _departments.Max(d => d.Id) + 1 : 1;
                if (department.CreatedDate == default)
                {
                    department.CreatedDate = DateTime.Now;
                }
                _departments.Add(department);
                TempData["Success"] = "Department created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        public IActionResult Edit(int id)
        {
            var department = _departments.FirstOrDefault(d => d.Id == id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        [HttpPost]
        public IActionResult Edit(int id, Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = _departments.FirstOrDefault(d => d.Id == id);
                if (existing != null)
                {
                    existing.Name = department.Name;
                    existing.Code = department.Code;
                    existing.Description = department.Description;
                    existing.Manager = department.Manager;
                    existing.EmployeeCount = department.EmployeeCount;
                    existing.IsActive = department.IsActive;
                    // Note: Preserving CreatedDate from existing record or updating it if needed
                    
                    TempData["Success"] = "Department updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }
            return View(department);
        }

        public IActionResult Details(int id)
        {
            var department = _departments.FirstOrDefault(d => d.Id == id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        [HttpPost] // Changed from implicit to explicit HttpPost for safety, though previously it was just 'Delete'
        public IActionResult Delete(int id)
        {
            var department = _departments.FirstOrDefault(d => d.Id == id);
            if (department != null)
            {
                _departments.Remove(department);
                TempData["Success"] = "Department deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
