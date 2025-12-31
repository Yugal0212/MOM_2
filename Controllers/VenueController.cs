using Microsoft.AspNetCore.Mvc;
using MOM.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOM.Controllers
{
    public class VenueController : Controller
    {
        private static List<Venue> _venues = new List<Venue>
        {
            new Venue { Id = 1, Name = "Board Room A", Location = "2nd Floor, Building A", Building = "Building A", RoomNumber = "201", Capacity = 20, VenueType = "Physical", Equipment = "Projector, Whiteboard, Conference Phone", CreatedDate = new DateTime(2020, 5, 10), IsActive = true },
            new Venue { Id = 2, Name = "Meeting Room B", Location = "1st Floor, Building B", Building = "Building B", RoomNumber = "105", Capacity = 15, VenueType = "Physical", Equipment = "TV Screen, Whiteboard", CreatedDate = new DateTime(2020, 6, 15), IsActive = true },
            new Venue { Id = 3, Name = "Executive Suite", Location = "3rd Floor, Building A", Building = "Building A", RoomNumber = "301", Capacity = 30, VenueType = "Physical", Equipment = "Video Conference, Projector, Sound System", CreatedDate = new DateTime(2019, 8, 20), IsActive = true },
            new Venue { Id = 4, Name = "Virtual Meeting Room 1", Location = "Online", Building = "Virtual", RoomNumber = "VM-01", Capacity = 100, VenueType = "Virtual", MeetingLink = "https://meet.company.com/room1", Equipment = "Video Conference System", CreatedDate = new DateTime(2020, 12, 1), IsActive = true },
            new Venue { Id = 5, Name = "Training Center", Location = "Ground Floor, Building C", Building = "Building C", RoomNumber = "G01", Capacity = 50, VenueType = "Physical", Equipment = "Projector, Computer Lab, Audio System", CreatedDate = new DateTime(2021, 2, 14), IsActive = true },
            new Venue { Id = 6, Name = "Open Space Area", Location = "2nd Floor, Building B", Building = "Building B", RoomNumber = "200", Capacity = 40, VenueType = "Physical", Equipment = "Movable Whiteboards, WiFi", CreatedDate = new DateTime(2021, 3, 8), IsActive = true }
        };

        public IActionResult Index()
        {
            return View(_venues);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Venue venue)
        {
            if (ModelState.IsValid)
            {
                venue.Id = _venues.Any() ? _venues.Max(v => v.Id) + 1 : 1;
                if (venue.CreatedDate == default)
                {
                    venue.CreatedDate = DateTime.Now;
                }
                _venues.Add(venue);
                TempData["Success"] = "Venue created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        public IActionResult Edit(int id)
        {
            var venue = _venues.FirstOrDefault(v => v.Id == id);
            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        [HttpPost]
        public IActionResult Edit(int id, Venue venue)
        {
            if (id != venue.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = _venues.FirstOrDefault(v => v.Id == id);
                if (existing != null)
                {
                    existing.Name = venue.Name;
                    existing.Location = venue.Location;
                    existing.Building = venue.Building;
                    existing.RoomNumber = venue.RoomNumber;
                    existing.Capacity = venue.Capacity;
                    existing.VenueType = venue.VenueType;
                    existing.MeetingLink = venue.MeetingLink;
                    existing.Equipment = venue.Equipment;
                    existing.IsActive = venue.IsActive;
                    
                    TempData["Success"] = "Venue updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }
            return View(venue);
        }

        public IActionResult Details(int id)
        {
            var venue = _venues.FirstOrDefault(v => v.Id == id);
            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var venue = _venues.FirstOrDefault(v => v.Id == id);
            if (venue != null)
            {
                _venues.Remove(venue);
                TempData["Success"] = "Venue deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
