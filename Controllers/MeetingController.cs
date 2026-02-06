using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using MOM.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace MOM.Controllers
{
    [Authorize]
    public class MeetingController : Controller
    {
        private readonly IConfiguration _configuration;

        public MeetingController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index(string searchTerm, int? meetingTypeId, int? departmentId, int? venueId, DateTime? startDate, DateTime? endDate)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            DataTable table = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "SP_Meeting_GetAll";
                
                SqlDataReader reader = cmd.ExecuteReader();
                table.Load(reader);
                reader.Close();
                
                // Get dropdowns
                DataTable typesTable = new DataTable();
                SqlCommand typesCmd = new SqlCommand();
                typesCmd.Connection = conn;
                typesCmd.CommandType = CommandType.StoredProcedure;
                typesCmd.CommandText = "SP_MeetingType_GetAll";
                SqlDataReader typesReader = typesCmd.ExecuteReader();
                typesTable.Load(typesReader);
                typesReader.Close();
                
                DataTable venuesTable = new DataTable();
                SqlCommand venuesCmd = new SqlCommand();
                venuesCmd.Connection = conn;
                venuesCmd.CommandType = CommandType.StoredProcedure;
                venuesCmd.CommandText = "SP_MeetingVenue_GetAll";
                SqlDataReader venuesReader = venuesCmd.ExecuteReader();
                venuesTable.Load(venuesReader);
                venuesReader.Close();
                
                DataTable deptsTable = new DataTable();
                SqlCommand deptsCmd = new SqlCommand();
                deptsCmd.Connection = conn;
                deptsCmd.CommandType = CommandType.StoredProcedure;
                deptsCmd.CommandText = "SP_Department_GetAll";
                SqlDataReader deptsReader = deptsCmd.ExecuteReader();
                deptsTable.Load(deptsReader);
                
                var types = new List<SelectListItem>();
                foreach (DataRow row in typesTable.Rows)
                {
                    types.Add(new SelectListItem { Value = row["MeetingTypeID"].ToString(), Text = row["MeetingTypeName"].ToString() });
                }
                
                var venues = new List<SelectListItem>();
                foreach (DataRow row in venuesTable.Rows)
                {
                    venues.Add(new SelectListItem { Value = row["MeetingVenueID"].ToString(), Text = row["MeetingVenueName"].ToString() });
                }
                
                var depts = new List<SelectListItem>();
                foreach (DataRow row in deptsTable.Rows)
                {
                    depts.Add(new SelectListItem { Value = row["DepartmentID"].ToString(), Text = row["DepartmentName"].ToString() });
                }
                
                ViewBag.MeetingTypes = new SelectList(types, "Value", "Text");
                ViewBag.Venues = new SelectList(venues, "Value", "Text");
                ViewBag.Departments = new SelectList(depts, "Value", "Text");
            }

            // Preserve search params
            ViewBag.CurrentSearch = searchTerm;
            ViewBag.CurrentMeetingTypeId = meetingTypeId;
            ViewBag.CurrentDepartmentId = departmentId;
            ViewBag.CurrentVenueId = venueId;
            ViewBag.CurrentStartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.CurrentEndDate = endDate?.ToString("yyyy-MM-dd");

            return View(table);
        }

        public IActionResult Schedule()
        {
            PopulateDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Schedule(Meeting meeting)
        {
            // Remove validation errors for NotMapped properties
            ModelState.Remove("Title");
            ModelState.Remove("MeetingTypeName");
            ModelState.Remove("VenueName");
            ModelState.Remove("DepartmentName");
            ModelState.Remove("Status");
            ModelState.Remove("Priority");
            
            if (ModelState.IsValid)
            {
                // Merge StartTime into MeetingDate
                meeting.MeetingDate = meeting.MeetingDate.Date.Add(meeting.StartTime);

                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_Meeting_Create";
                    
                    cmd.Parameters.AddWithValue("@MeetingDate", meeting.MeetingDate);
                    cmd.Parameters.AddWithValue("@MeetingVenueID", meeting.MeetingVenueId);
                    cmd.Parameters.AddWithValue("@MeetingTypeID", meeting.MeetingTypeId);
                    cmd.Parameters.AddWithValue("@DepartmentID", meeting.DepartmentId);
                    cmd.Parameters.AddWithValue("@MeetingDescription", meeting.MeetingDescription ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DocumentPath", meeting.DocumentPath ?? (object)DBNull.Value);
                    
                    cmd.ExecuteNonQuery();
                }
                
                TempData["Success"] = "Meeting scheduled successfully!";
                return RedirectToAction(nameof(Index));
            }

            PopulateDropdowns(meeting);
            return View(meeting);
        }

        public IActionResult Details(int id)
        {
            Meeting meeting = null;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "SP_Meeting_GetByID";
                cmd.Parameters.AddWithValue("@MeetingId", id);
                
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    meeting = new Meeting
                    {
                        MeetingId = Convert.ToInt32(reader["MeetingId"]),
                        MeetingDate = Convert.ToDateTime(reader["MeetingDate"]),
                        MeetingVenueId = Convert.ToInt32(reader["MeetingVenueId"]),
                        MeetingTypeId = Convert.ToInt32(reader["MeetingTypeId"]),
                        DepartmentId = Convert.ToInt32(reader["DepartmentId"]),
                        MeetingDescription = reader["MeetingDescription"] != DBNull.Value ? reader["MeetingDescription"].ToString() : null,
                        DocumentPath = reader["DocumentPath"] != DBNull.Value ? reader["DocumentPath"].ToString() : null,
                        Created = Convert.ToDateTime(reader["Created"]),
                        Modified = Convert.ToDateTime(reader["Modified"]),
                        IsCancelled = reader["IsCancelled"] != DBNull.Value ? Convert.ToBoolean(reader["IsCancelled"]) : (bool?)null,
                        CancellationDateTime = reader["CancellationDateTime"] != DBNull.Value ? Convert.ToDateTime(reader["CancellationDateTime"]) : (DateTime?)null,
                        CancellationReason = reader["CancellationReason"] != DBNull.Value ? reader["CancellationReason"].ToString() : null,
                        MeetingTypeName = reader["MeetingTypeName"] != DBNull.Value ? reader["MeetingTypeName"].ToString() : null,
                        VenueName = reader["MeetingVenueName"] != DBNull.Value ? reader["MeetingVenueName"].ToString() : null,
                        DepartmentName = reader["DepartmentName"] != DBNull.Value ? reader["DepartmentName"].ToString() : null
                    };
                    meeting.StartTime = meeting.MeetingDate.TimeOfDay;
                }
            }
            
            if (meeting == null)
            {
                return NotFound();
            }

            return View(meeting);
        }

        public IActionResult Edit(int id)
        {
            Meeting meeting = null;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "SP_Meeting_GetByID";
                cmd.Parameters.AddWithValue("@MeetingId", id);
                
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    meeting = new Meeting
                    {
                        MeetingId = Convert.ToInt32(reader["MeetingId"]),
                        MeetingDate = Convert.ToDateTime(reader["MeetingDate"]),
                        MeetingVenueId = Convert.ToInt32(reader["MeetingVenueId"]),
                        MeetingTypeId = Convert.ToInt32(reader["MeetingTypeId"]),
                        DepartmentId = Convert.ToInt32(reader["DepartmentId"]),
                        MeetingDescription = reader["MeetingDescription"] != DBNull.Value ? reader["MeetingDescription"].ToString() : null,
                        DocumentPath = reader["DocumentPath"] != DBNull.Value ? reader["DocumentPath"].ToString() : null,
                        Created = Convert.ToDateTime(reader["Created"]),
                        Modified = Convert.ToDateTime(reader["Modified"])
                    };
                    meeting.StartTime = meeting.MeetingDate.TimeOfDay;
                }
            }

            if (meeting == null)
            {
                return NotFound();
            }

            PopulateDropdowns(meeting);
            return View(meeting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Meeting meeting)
        {
            if (id != meeting.MeetingId)
            {
                return NotFound();
            }

            // Remove validation errors for NotMapped properties
            ModelState.Remove("Title");
            ModelState.Remove("MeetingTypeName");
            ModelState.Remove("VenueName");
            ModelState.Remove("DepartmentName");
            ModelState.Remove("Status");
            ModelState.Remove("Priority");

            if (ModelState.IsValid)
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_Meeting_Update";
                    
                    cmd.Parameters.AddWithValue("@MeetingId", meeting.MeetingId);
                    cmd.Parameters.AddWithValue("@MeetingDate", meeting.MeetingDate.Date.Add(meeting.StartTime));
                    cmd.Parameters.AddWithValue("@MeetingVenueId", meeting.MeetingVenueId);
                    cmd.Parameters.AddWithValue("@MeetingTypeId", meeting.MeetingTypeId);
                    cmd.Parameters.AddWithValue("@DepartmentId", meeting.DepartmentId);
                    cmd.Parameters.AddWithValue("@MeetingDescription", meeting.MeetingDescription ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DocumentPath", meeting.DocumentPath ?? (object)DBNull.Value);
                    
                    cmd.ExecuteNonQuery();
                }
                
                TempData["Success"] = "Meeting updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            PopulateDropdowns(meeting);
            return View(meeting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_Meeting_Delete";
                    
                    cmd.Parameters.AddWithValue("@MeetingId", id);
                    
                    cmd.ExecuteNonQuery();
                }
                
                TempData["Success"] = "Meeting deleted successfully!";
            }
            catch (SqlException ex)
            {
                TempData["Error"] = "Error deleting meeting: " + ex.Message;
            }
            
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(int id, string status)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                
                if (status == "Cancelled")
                {
                    cmd.CommandText = "UPDATE MOM_Meetings SET IsCancelled = 1, CancellationDateTime = GETDATE(), Modified = GETDATE() WHERE MeetingId = @MeetingId";
                }
                else
                {
                    cmd.CommandText = "UPDATE MOM_Meetings SET IsCancelled = 0, CancellationDateTime = NULL, Modified = GETDATE() WHERE MeetingId = @MeetingId";
                }
                
                cmd.Parameters.AddWithValue("@MeetingId", id);
                cmd.ExecuteNonQuery();
            }
            
            TempData["Success"] = $"Meeting status updated to {status}!";
            return RedirectToAction(nameof(Index));
        }

        private void PopulateDropdowns(Meeting selectedMeeting = null)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                
                DataTable typesTable = new DataTable();
                SqlCommand typesCmd = new SqlCommand();
                typesCmd.Connection = conn;
                typesCmd.CommandType = CommandType.StoredProcedure;
                typesCmd.CommandText = "SP_MeetingType_GetAll";
                SqlDataReader typesReader = typesCmd.ExecuteReader();
                typesTable.Load(typesReader);
                typesReader.Close();
                
                DataTable venuesTable = new DataTable();
                SqlCommand venuesCmd = new SqlCommand();
                venuesCmd.Connection = conn;
                venuesCmd.CommandType = CommandType.StoredProcedure;
                venuesCmd.CommandText = "SP_MeetingVenue_GetAll";
                SqlDataReader venuesReader = venuesCmd.ExecuteReader();
                venuesTable.Load(venuesReader);
                venuesReader.Close();
                
                DataTable deptsTable = new DataTable();
                SqlCommand deptsCmd = new SqlCommand();
                deptsCmd.Connection = conn;
                deptsCmd.CommandType = CommandType.StoredProcedure;
                deptsCmd.CommandText = "SP_Department_GetAll";
                SqlDataReader deptsReader = deptsCmd.ExecuteReader();
                deptsTable.Load(deptsReader);
                
                var types = new List<SelectListItem>();
                foreach (DataRow row in typesTable.Rows)
                {
                    types.Add(new SelectListItem 
                    { 
                        Value = row["MeetingTypeID"].ToString(), 
                        Text = row["MeetingTypeName"].ToString(),
                        Selected = selectedMeeting != null && row["MeetingTypeID"].ToString() == selectedMeeting.MeetingTypeId.ToString()
                    });
                }
                
                var venues = new List<SelectListItem>();
                foreach (DataRow row in venuesTable.Rows)
                {
                    venues.Add(new SelectListItem 
                    { 
                        Value = row["MeetingVenueID"].ToString(), 
                        Text = row["MeetingVenueName"].ToString(),
                        Selected = selectedMeeting != null && row["MeetingVenueID"].ToString() == selectedMeeting.MeetingVenueId.ToString()
                    });
                }
                
                var depts = new List<SelectListItem>();
                foreach (DataRow row in deptsTable.Rows)
                {
                    depts.Add(new SelectListItem 
                    { 
                        Value = row["DepartmentID"].ToString(), 
                        Text = row["DepartmentName"].ToString(),
                        Selected = selectedMeeting != null && row["DepartmentID"].ToString() == selectedMeeting.DepartmentId.ToString()
                    });
                }
                
                ViewBag.MeetingTypes = types;
                ViewBag.Venues = venues;
                ViewBag.Departments = depts;
            }
        }
    }
}
