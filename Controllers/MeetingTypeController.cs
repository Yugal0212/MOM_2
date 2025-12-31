using Microsoft.AspNetCore.Mvc;
using MOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOM.Controllers
{
    public class MeetingTypeController : Controller
    {
        private static List<MeetingType> _meetingTypes = new List<MeetingType>
        {
            new MeetingType { Id = 1, Name = "Board Meeting", Code = "BM", Description = "Executive board meeting", DefaultDuration = 120, CreatedDate = new DateTime(2023, 1, 15), IsActive = true },
            new MeetingType { Id = 2, Name = "Team Standup", Code = "TS", Description = "Daily team synchronization", DefaultDuration = 30, CreatedDate = new DateTime(2023, 2, 10), IsActive = true },
            new MeetingType { Id = 3, Name = "Project Planning", Code = "PP", Description = "Project kickoff and planning session", DefaultDuration = 90, CreatedDate = new DateTime(2023, 3, 5), IsActive = true },
            new MeetingType { Id = 4, Name = "Budget Review", Code = "BR", Description = "Financial review and budget planning", DefaultDuration = 120, CreatedDate = new DateTime(2023, 4, 12), IsActive = true },
            new MeetingType { Id = 5, Name = "Client Meeting", Code = "CM", Description = "Client presentation and discussion", DefaultDuration = 60, CreatedDate = new DateTime(2023, 5, 8), IsActive = true },
            new MeetingType { Id = 6, Name = "Training Session", Code = "TR", Description = "Staff training and development", DefaultDuration = 120, CreatedDate = new DateTime(2023, 6, 20), IsActive = true }
        };

        public IActionResult Index()
        {
            return View(_meetingTypes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(MeetingType meetingType)
        {
            if (ModelState.IsValid)
            {
                meetingType.Id = _meetingTypes.Any() ? _meetingTypes.Max(d => d.Id) + 1 : 1;
                if (meetingType.CreatedDate == default)
                {
                    meetingType.CreatedDate = DateTime.Now;
                }
                _meetingTypes.Add(meetingType);
                TempData["Success"] = "Meeting Type created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(meetingType);
        }

        public IActionResult Edit(int id)
        {
            var meetingType = _meetingTypes.FirstOrDefault(m => m.Id == id);
            if (meetingType == null)
            {
                return NotFound();
            }
            return View(meetingType);
        }

        [HttpPost]
        public IActionResult Edit(int id, MeetingType meetingType)
        {
            if (id != meetingType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = _meetingTypes.FirstOrDefault(m => m.Id == id);
                if (existing != null)
                {
                    existing.Name = meetingType.Name;
                    existing.Code = meetingType.Code;
                    existing.Description = meetingType.Description;
                    existing.DefaultDuration = meetingType.DefaultDuration;
                    existing.IsActive = meetingType.IsActive;
                    
                    TempData["Success"] = "Meeting Type updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }
            return View(meetingType);
        }

        public IActionResult Details(int id)
        {
            var meetingType = _meetingTypes.FirstOrDefault(m => m.Id == id);
            if (meetingType == null)
            {
                return NotFound();
            }
            return View(meetingType);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var meetingType = _meetingTypes.FirstOrDefault(m => m.Id == id);
            if (meetingType != null)
            {
                _meetingTypes.Remove(meetingType);
                TempData["Success"] = "Meeting Type deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
