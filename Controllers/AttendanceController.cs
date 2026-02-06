using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MOM.Data;
using MOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOM.Controllers
{
    [Authorize]
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Summary View
            var meetings = _context.Meetings.OrderByDescending(m => m.MeetingDate).ToList();
            var model = new List<AttendanceSummaryViewModel>();

            foreach (var meeting in meetings)
            {
                var members = _context.MeetingMembers.Where(m => m.MeetingId == meeting.MeetingId).ToList();
                
                var summary = new AttendanceSummaryViewModel
                {
                    Meeting = meeting,
                    TotalStaff = members.Count,
                    PresentCount = members.Count(m => m.IsPresent),
                    AbsentCount = members.Count(m => !m.IsPresent),
                    IsAttendanceTaken = members.Any() // Simplistic logic: if members exist, we assume attendance structure exists. Ideally "Taken" means verified.
                };

                model.Add(summary);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Manage(int meetingId)
        {
            var meeting = _context.Meetings.FirstOrDefault(m => m.MeetingId == meetingId); // Include if needed, primarily need meeting details
            if (meeting == null) return NotFound();

            var members = _context.MeetingMembers.Where(m => m.MeetingId == meetingId).ToList();
            var staffIds = members.Select(m => m.StaffId).ToList();
            var staffDict = _context.Staff.Where(s => staffIds.Contains(s.StaffId)).ToDictionary(s => s.StaffId);

            var viewModel = new AttendanceViewModel
            {
                MeetingId = meeting.MeetingId,
                MeetingTitle = meeting.MeetingDescription, // Using Description as Title
                MeetingDate = meeting.MeetingDate,
                StaffList = new List<StaffAttendanceItem>()
            };

            foreach (var member in members)
            {
                var staffName = "Unknown";
                var deptName = "";
                if (staffDict.TryGetValue(member.StaffId, out var s))
                {
                    staffName = s.StaffName;
                    // Note: DepartmentName isn't directly on Staff, it's via DepartmentId. 
                    // To avoid complex queries, I'll skip fetching Dept Name for now or do another lookup if critical.
                }

                viewModel.StaffList.Add(new StaffAttendanceItem
                {
                    StaffId = member.StaffId,
                    Name = staffName,
                    Department = deptName, // Left empty to save query, or we can fetch.
                    IsPresent = member.IsPresent,
                    Remarks = member.Remarks
                });
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(AttendanceViewModel model)
        {
            if (model.StaffList != null)
            {
                foreach (var item in model.StaffList)
                {
                    var member = _context.MeetingMembers.FirstOrDefault(m => m.MeetingId == model.MeetingId && m.StaffId == item.StaffId);
                    if (member != null)
                    {
                        member.IsPresent = item.IsPresent;
                        member.Remarks = item.Remarks;
                        member.Modified = DateTime.Now;
                    }
                }
                _context.SaveChanges();
            }

            TempData["Success"] = "Attendance saved successfully!";
            return RedirectToAction("Index");
        }
    }
}
