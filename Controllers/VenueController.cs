using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM.Models;
using System;
using System.Data;

namespace MOM.Controllers
{
    [Authorize]
    public class VenueController : Controller
    {
        private readonly IConfiguration _configuration;

        public VenueController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "SP_MeetingVenue_GetAll";
            SqlDataReader reader = command.ExecuteReader();

            DataTable table = new DataTable();
            table.Load(reader);
            connection.Close();

            return View(table);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(MeetingVenue venue)
        {
            if (ModelState.IsValid)
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SP_MeetingVenue_Create";
                command.Parameters.AddWithValue("@MeetingVenueName", venue.MeetingVenueName);
                
                command.ExecuteNonQuery();
                connection.Close();
                
                TempData["Success"] = "Venue created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        public IActionResult Edit(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "SP_MeetingVenue_GetByID";
            command.Parameters.AddWithValue("@MeetingVenueID", id);
            
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            connection.Close();
            
            if (table.Rows.Count == 0)
            {
                return NotFound();
            }
            
            DataRow row = table.Rows[0];
            MeetingVenue venue = new MeetingVenue
            {
                MeetingVenueId = Convert.ToInt32(row["MeetingVenueID"]),
                MeetingVenueName = row["MeetingVenueName"].ToString()
            };
            
            return View(venue);
        }

        [HttpPost]
        public IActionResult Edit(int id, MeetingVenue venue)
        {
            if (id != venue.MeetingVenueId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SP_MeetingVenue_Update";
                command.Parameters.AddWithValue("@MeetingVenueID", venue.MeetingVenueId);
                command.Parameters.AddWithValue("@MeetingVenueName", venue.MeetingVenueName);
                
                command.ExecuteNonQuery();
                connection.Close();
                
                TempData["Success"] = "Venue updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        public IActionResult Details(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "SP_MeetingVenue_GetByID";
            command.Parameters.AddWithValue("@MeetingVenueID", id);
            
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            connection.Close();
            
            if (table.Rows.Count == 0)
            {
                return NotFound();
            }
            
            DataRow row = table.Rows[0];
            MeetingVenue venue = new MeetingVenue
            {
                MeetingVenueId = Convert.ToInt32(row["MeetingVenueID"]),
                MeetingVenueName = row["MeetingVenueName"].ToString()
            };
            
            return View(venue);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SP_MeetingVenue_Delete";
                command.Parameters.AddWithValue("@MeetingVenueID", id);
                
                command.ExecuteNonQuery();
                connection.Close();
                
                TempData["Success"] = "Venue deleted successfully!";
            }
            catch(SqlException)
            {
                TempData["Error"] = "Cannot delete venue as it is referenced by meetings.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
