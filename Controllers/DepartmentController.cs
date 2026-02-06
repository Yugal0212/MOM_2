using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM.Models;
using System;
using System.Data;

namespace MOM.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly IConfiguration _configuration;

        public DepartmentController(IConfiguration configuration)
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
            command.CommandText = "SP_Department_GetAll";
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
        public IActionResult Create(Department department)
        {
            if (ModelState.IsValid)
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SP_Department_Create";
                command.Parameters.AddWithValue("@DepartmentName", department.DepartmentName);
                
                command.ExecuteNonQuery();
                connection.Close();
                
                TempData["Success"] = "Department created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        public IActionResult Edit(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "SP_Department_GetByID";
            command.Parameters.AddWithValue("@DepartmentID", id);
            
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            connection.Close();
            
            if (table.Rows.Count == 0)
            {
                return NotFound();
            }
            
            DataRow row = table.Rows[0];
            Department department = new Department
            {
                DepartmentId = Convert.ToInt32(row["DepartmentID"]),
                DepartmentName = row["DepartmentName"].ToString()
            };
            
            return View(department);
        }

        [HttpPost]
        public IActionResult Edit(int id, Department department)
        {
            if (id != department.DepartmentId)
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
                command.CommandText = "SP_Department_Update";
                command.Parameters.AddWithValue("@DepartmentID", department.DepartmentId);
                command.Parameters.AddWithValue("@DepartmentName", department.DepartmentName);
                
                command.ExecuteNonQuery();
                connection.Close();
                
                TempData["Success"] = "Department updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        public IActionResult Details(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "SP_Department_GetByID";
            command.Parameters.AddWithValue("@DepartmentID", id);
            
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            connection.Close();
            
            if (table.Rows.Count == 0)
            {
                return NotFound();
            }
            
            DataRow row = table.Rows[0];
            Department department = new Department
            {
                DepartmentId = Convert.ToInt32(row["DepartmentID"]),
                DepartmentName = row["DepartmentName"].ToString()
            };
            
            return View(department);
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
                command.CommandText = "SP_Department_Delete";
                command.Parameters.AddWithValue("@DepartmentID", id);
                
                command.ExecuteNonQuery();
                connection.Close();
                
                TempData["Success"] = "Department deleted successfully!";
            }
            catch(SqlException)
            {
                TempData["Error"] = "Cannot delete department as it is referenced by staff or meetings.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
