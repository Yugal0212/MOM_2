using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM.Models;
using System;
using System.Data;

namespace MOM.Controllers
{
    [Authorize]
    public class MeetingTypeController : Controller
    {
        private readonly IConfiguration _configuration;

        public MeetingTypeController(IConfiguration configuration)
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
            command.CommandText = "SP_MeetingType_GetAll";
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
        public IActionResult Create(MeetingType meetingType)
        {
            if (ModelState.IsValid)
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SP_MeetingType_Create";
                command.Parameters.AddWithValue("@MeetingTypeName", meetingType.MeetingTypeName);
                command.Parameters.AddWithValue("@Remarks", meetingType.Remarks ?? (object)DBNull.Value);
                
                command.ExecuteNonQuery();
                connection.Close();
                
                TempData["Success"] = "Meeting Type created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(meetingType);
        }

        public IActionResult Edit(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "SP_MeetingType_GetByID";
            command.Parameters.AddWithValue("@MeetingTypeID", id);
            
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            connection.Close();
            
            if (table.Rows.Count == 0)
            {
                return NotFound();
            }
            
            DataRow row = table.Rows[0];
            MeetingType meetingType = new MeetingType
            {
                MeetingTypeId = Convert.ToInt32(row["MeetingTypeID"]),
                MeetingTypeName = row["MeetingTypeName"].ToString(),
                Remarks = row["Remarks"] != DBNull.Value ? row["Remarks"].ToString() : null
            };
            
            return View(meetingType);
        }

        [HttpPost]
        public IActionResult Edit(int id, MeetingType meetingType)
        {
            if (id != meetingType.MeetingTypeId)
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
                command.CommandText = "SP_MeetingType_Update";
                command.Parameters.AddWithValue("@MeetingTypeID", meetingType.MeetingTypeId);
                command.Parameters.AddWithValue("@MeetingTypeName", meetingType.MeetingTypeName);
                command.Parameters.AddWithValue("@Remarks", meetingType.Remarks ?? (object)DBNull.Value);
                
                command.ExecuteNonQuery();
                connection.Close();
                
                TempData["Success"] = "Meeting Type updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(meetingType);
        }

        public IActionResult Details(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "SP_MeetingType_GetByID";
            command.Parameters.AddWithValue("@MeetingTypeID", id);
            
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            connection.Close();
            
            if (table.Rows.Count == 0)
            {
                return NotFound();
            }
            
            DataRow row = table.Rows[0];
            MeetingType meetingType = new MeetingType
            {
                MeetingTypeId = Convert.ToInt32(row["MeetingTypeID"]),
                MeetingTypeName = row["MeetingTypeName"].ToString(),
                Remarks = row["Remarks"] != DBNull.Value ? row["Remarks"].ToString() : null
            };
            
            return View(meetingType);
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
                command.CommandText = "SP_MeetingType_Delete";
                command.Parameters.AddWithValue("@MeetingTypeID", id);
                
                command.ExecuteNonQuery();
                connection.Close();
                
                TempData["Success"] = "Meeting Type deleted successfully!";
            }
            catch(SqlException)
            {
                TempData["Error"] = "Cannot delete meeting type as it is referenced by meetings.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
