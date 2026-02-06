using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MOM.Data;
using MOM.Models;
using System;
using System.Linq;

namespace MOM.Controllers
{
    [Authorize]
    public class MeetingMemberController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MeetingMemberController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? meetingId)
        {
            var query = _context.MeetingMembers.AsQueryable();
            if (meetingId.HasValue)
            {
                query = query.Where(m => m.MeetingId == meetingId);
            }
            
            var list = query.OrderBy(m => m.MeetingMemberId).ToList();
            
            var meetingIds = list.Select(m => m.MeetingId).Distinct().ToList();
            var staffIds = list.Select(m => m.StaffId).Distinct().ToList();

            var meetings = _context.Meetings.Where(m => meetingIds.Contains(m.MeetingId))
                            .Select(m => new { m.MeetingId, m.MeetingDescription, m.MeetingDate }) // Description acts as Title usually
                            .ToDictionary(m => m.MeetingId, m => m.MeetingDescription ?? $"Meeting {m.MeetingId}");

            var staffs = _context.Staff.Where(s => staffIds.Contains(s.StaffId))
                            .ToDictionary(s => s.StaffId, s => s.StaffName);

            foreach(var item in list)
            {
                if(meetings.ContainsKey(item.MeetingId)) item.MeetingTitle = meetings[item.MeetingId];
                if(staffs.ContainsKey(item.StaffId)) item.StaffName = staffs[item.StaffId];
            }

            return View(list);
        }

        public IActionResult Create()
        {
            ViewBag.Meetings = new SelectList(_context.Meetings, "MeetingId", "MeetingDescription"); 
           
            
            ViewBag.Staff = new SelectList(_context.Staff, "StaffId", "StaffName");
            return View();
        }

        [HttpPost]
        public IActionResult Create(MeetingMember meetingMember)
        {
            if (ModelState.IsValid)
            {
                // Validation: Check if member already exists in meeting
                if (_context.MeetingMembers.Any(m => m.MeetingId == meetingMember.MeetingId && m.StaffId == meetingMember.StaffId))
                {
                    ModelState.AddModelError("", "This staff member is already added to the meeting.");
                }
                else
                {
                    meetingMember.Created = DateTime.Now;
                    meetingMember.Modified = DateTime.Now;
                    
                    _context.MeetingMembers.Add(meetingMember);
                    _context.SaveChanges();

                    TempData["Success"] = "Meeting Member added successfully!";
                    return RedirectToAction(nameof(Index), new { meetingId = meetingMember.MeetingId });
                }
            }
            
            ViewBag.Meetings = new SelectList(_context.Meetings, "MeetingId", "MeetingDescription", meetingMember.MeetingId);
            ViewBag.Staff = new SelectList(_context.Staff, "StaffId", "StaffName", meetingMember.StaffId);
            return View(meetingMember);
        }

        public IActionResult Edit(int id)
        {
            var meetingMember = _context.MeetingMembers.FirstOrDefault(m => m.MeetingMemberId == id);
            if (meetingMember == null)
            {
                return NotFound();
            }
            
            ViewBag.Meetings = new SelectList(_context.Meetings, "MeetingId", "MeetingDescription", meetingMember.MeetingId);
            ViewBag.Staff = new SelectList(_context.Staff, "StaffId", "StaffName", meetingMember.StaffId);
            
            return View(meetingMember);
        }

        [HttpPost]
        public IActionResult Edit(int id, MeetingMember meetingMember)
        {
            if (id != meetingMember.MeetingMemberId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = _context.MeetingMembers.FirstOrDefault(m => m.MeetingMemberId == id);
                if (existing != null)
                {
                    // Check duplicates if needed
                    bool isDuplicate = false;
                    if (existing.MeetingId != meetingMember.MeetingId || existing.StaffId != meetingMember.StaffId)
                    {
                        if (_context.MeetingMembers.Any(m => m.MeetingId == meetingMember.MeetingId && m.StaffId == meetingMember.StaffId && m.MeetingMemberId != id))
                        {
                            isDuplicate = true;
                        }
                    }

                    if (isDuplicate)
                    {
                        ModelState.AddModelError("", "This staff member is already added to the selected meeting.");
                    }
                    else
                    {
                        existing.MeetingId = meetingMember.MeetingId;
                        existing.StaffId = meetingMember.StaffId;
                        existing.IsPresent = meetingMember.IsPresent;
                        existing.Remarks = meetingMember.Remarks;
                        existing.Modified = DateTime.Now;
                        
                        _context.SaveChanges();
                        
                        TempData["Success"] = "Meeting Member updated successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            
            ViewBag.Meetings = new SelectList(_context.Meetings, "MeetingId", "MeetingDescription", meetingMember.MeetingId);
            ViewBag.Staff = new SelectList(_context.Staff, "StaffId", "StaffName", meetingMember.StaffId);
            return View(meetingMember);
        }

        public IActionResult Details(int id)
        {
            var meetingMember = _context.MeetingMembers.FirstOrDefault(m => m.MeetingMemberId == id);
            if (meetingMember == null)
            {
                return NotFound();
            }
            // Populate names
            var meeting = _context.Meetings.Find(meetingMember.MeetingId);
            var staff = _context.Staff.Find(meetingMember.StaffId);
            meetingMember.MeetingTitle = meeting?.MeetingDescription;
            meetingMember.StaffName = staff?.StaffName;

            return View(meetingMember);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var meetingMember = _context.MeetingMembers.FirstOrDefault(m => m.MeetingMemberId == id);
            if (meetingMember != null)
            {
                _context.MeetingMembers.Remove(meetingMember);
                _context.SaveChanges();
                TempData["Success"] = "Meeting Member deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
