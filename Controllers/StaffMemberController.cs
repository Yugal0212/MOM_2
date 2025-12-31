using Microsoft.AspNetCore.Mvc;
using MOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOM.Controllers
{
    public class StaffMemberController : Controller
    {
        private static List<StaffMember> _staffMembers = new List<StaffMember>
        {
            new StaffMember { Id = 1, FirstName = "John", LastName = "Smith", Email = "john.smith@company.com", Phone = "+1(555)123-4567", Department = "Human Resources", Position = "HR Manager", EmployeeId = "EMP001", JoinedDate = new DateTime(2020, 3, 15), IsActive = true },
            new StaffMember { Id = 2, FirstName = "Sarah", LastName = "Johnson", Email = "sarah.johnson@company.com", Phone = "+1(555)123-4568", Department = "Information Technology", Position = "IT Director", EmployeeId = "EMP002", JoinedDate = new DateTime(2019, 6, 20), IsActive = true },
            new StaffMember { Id = 3, FirstName = "Michael", LastName = "Brown", Email = "michael.brown@company.com", Phone = "+1(555)123-4569", Department = "Finance", Position = "Finance Manager", EmployeeId = "EMP003", JoinedDate = new DateTime(2018, 8, 10), IsActive = true },
            new StaffMember { Id = 4, FirstName = "Emily", LastName = "Davis", Email = "emily.davis@company.com", Phone = "+1(555)123-4570", Department = "Marketing", Position = "Marketing Manager", EmployeeId = "EMP004", JoinedDate = new DateTime(2021, 4, 5), IsActive = true },
            new StaffMember { Id = 5, FirstName = "David", LastName = "Wilson", Email = "david.wilson@company.com", Phone = "+1(555)123-4571", Department = "Operations", Position = "Operations Lead", EmployeeId = "EMP005", JoinedDate = new DateTime(2017, 9, 12), IsActive = true },
            new StaffMember { Id = 6, FirstName = "Jessica", LastName = "Martinez", Email = "jessica.martinez@company.com", Phone = "+1(555)123-4572", Department = "Sales", Position = "Sales Director", EmployeeId = "EMP006", JoinedDate = new DateTime(2020, 1, 8), IsActive = true }
        };

        public IActionResult Index()
        {
            return View(_staffMembers.OrderBy(s => s.LastName).ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(StaffMember staffMember)
        {
            if (ModelState.IsValid)
            {
                staffMember.Id = _staffMembers.Any() ? _staffMembers.Max(s => s.Id) + 1 : 1;
                if (staffMember.JoinedDate == default)
                {
                    staffMember.JoinedDate = DateTime.Now;
                }
                _staffMembers.Add(staffMember);
                TempData["Success"] = "Staff Member added successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(staffMember);
        }

        public IActionResult Edit(int id)
        {
            var staffMember = _staffMembers.FirstOrDefault(s => s.Id == id);
            if (staffMember == null)
            {
                return NotFound();
            }
            return View(staffMember);
        }

        [HttpPost]
        public IActionResult Edit(int id, StaffMember staffMember)
        {
            if (id != staffMember.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = _staffMembers.FirstOrDefault(s => s.Id == id);
                if (existing != null)
                {
                    existing.FirstName = staffMember.FirstName;
                    existing.LastName = staffMember.LastName;
                    existing.Email = staffMember.Email;
                    existing.Phone = staffMember.Phone;
                    existing.Department = staffMember.Department;
                    existing.Position = staffMember.Position;
                    existing.EmployeeId = staffMember.EmployeeId;
                    existing.IsActive = staffMember.IsActive;
                    
                    TempData["Success"] = "Staff Member updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }
            return View(staffMember);
        }

        public IActionResult Details(int id)
        {
            var staffMember = _staffMembers.FirstOrDefault(s => s.Id == id);
            if (staffMember == null)
            {
                return NotFound();
            }
            return View(staffMember);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var staffMember = _staffMembers.FirstOrDefault(s => s.Id == id);
            if (staffMember != null)
            {
                _staffMembers.Remove(staffMember);
                TempData["Success"] = "Staff Member deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
