using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using MOM.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MOM.Controllers
{
    [Authorize]
    public class StaffMemberController : Controller
    {
        private readonly IConfiguration _configuration;

        public StaffMemberController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static List<Staff> _staffMembers = new List<Staff>();

        public IActionResult Index(string searchTerm, int? departmentId)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SP_Staff_GetAll";
                
                SqlDataReader reader = command.ExecuteReader();
                DataTable table = new DataTable();
                table.Load(reader);
                reader.Close();
                
                // Get departments for dropdown
                SqlCommand deptCmd = connection.CreateCommand();
                deptCmd.CommandType = CommandType.StoredProcedure;
                deptCmd.CommandText = "SP_Department_GetAll";
                SqlDataReader deptReader = deptCmd.ExecuteReader();
                DataTable deptTable = new DataTable();
                deptTable.Load(deptReader);
                deptReader.Close();
                connection.Close();

                // Create SelectList - ensure we have data
                if (deptTable.Rows.Count > 0)
                {
                    var departmentList = new List<SelectListItem>();
                    foreach (DataRow row in deptTable.Rows)
                    {
                        departmentList.Add(new SelectListItem
                        {
                            Value = row["DepartmentID"].ToString(),
                            Text = row["DepartmentName"].ToString()
                        });
                    }
                    ViewBag.Departments = new SelectList(departmentList, "Value", "Text");
                }
                else
                {
                    ViewBag.Departments = new SelectList(new List<SelectListItem>());
                }
                
                ViewBag.CurrentSearch = searchTerm;
                ViewBag.CurrentDepartmentId = departmentId;
                
                return View(table);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading staff: {ex.Message}";
                return View(new DataTable());
            }
        }

        public IActionResult Create()
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
            
            ViewBag.Departments = new SelectList(table.Rows.Cast<DataRow>(), "DepartmentID", "DepartmentName");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Staff staff)
        {
            if (ModelState.IsValid)
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SP_Staff_Create";
                command.Parameters.AddWithValue("@DepartmentID", staff.DepartmentId);
                command.Parameters.AddWithValue("@StaffName", staff.StaffName);
                command.Parameters.AddWithValue("@MobileNo", staff.MobileNo);
                command.Parameters.AddWithValue("@EmailAddress", staff.EmailAddress);
                command.Parameters.AddWithValue("@Remarks", staff.Remarks ?? (object)DBNull.Value);
                
                command.ExecuteNonQuery();
                connection.Close();
                
                TempData["Success"] = "Staff Member added successfully!";
                return RedirectToAction(nameof(Index));
            }
            
            string connString = _configuration.GetConnectionString("DefaultConnection");
            SqlConnection conn = new SqlConnection(connString);
            conn.Open();
            
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SP_Department_GetAll";
            
            SqlDataReader rdr = cmd.ExecuteReader();
            DataTable tbl = new DataTable();
            tbl.Load(rdr);
            conn.Close();
            
            ViewBag.Departments = new SelectList(tbl.Rows.Cast<DataRow>(), "DepartmentID", "DepartmentName", staff.DepartmentId);
            return View(staff);
        }

        public IActionResult Edit(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "SP_Staff_GetByID";
            command.Parameters.AddWithValue("@StaffID", id);
            
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            reader.Close();
            
            if (table.Rows.Count == 0)
            {
                connection.Close();
                return NotFound();
            }
            
            DataRow row = table.Rows[0];
            Staff staff = new Staff
            {
                StaffId = Convert.ToInt32(row["StaffID"]),
                DepartmentId = Convert.ToInt32(row["DepartmentID"]),
                StaffName = row["StaffName"].ToString(),
                MobileNo = row["MobileNo"].ToString(),
                EmailAddress = row["EmailAddress"].ToString(),
                Remarks = row["Remarks"] != DBNull.Value ? row["Remarks"].ToString() : null
            };
            
            SqlCommand deptCmd = connection.CreateCommand();
            deptCmd.CommandType = CommandType.StoredProcedure;
            deptCmd.CommandText = "sp_Department_GetAll";
            SqlDataReader deptReader = deptCmd.ExecuteReader();
            DataTable deptTable = new DataTable();
            deptTable.Load(deptReader);
            connection.Close();
            
            ViewBag.Departments = new SelectList(deptTable.Rows.Cast<DataRow>(), "DepartmentID", "DepartmentName", staff.DepartmentId);
            return View(staff);
        }

        [HttpPost]
        public IActionResult Edit(int id, Staff staff)
        {
            if (id != staff.StaffId)
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
                command.CommandText = "SP_Staff_Update";
                command.Parameters.AddWithValue("@StaffID", staff.StaffId);
                command.Parameters.AddWithValue("@DepartmentID", staff.DepartmentId);
                command.Parameters.AddWithValue("@StaffName", staff.StaffName);
                command.Parameters.AddWithValue("@MobileNo", staff.MobileNo);
                command.Parameters.AddWithValue("@EmailAddress", staff.EmailAddress);
                command.Parameters.AddWithValue("@Remarks", staff.Remarks ?? (object)DBNull.Value);
                
                command.ExecuteNonQuery();
                connection.Close();
                
                TempData["Success"] = "Staff Member updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            
            string connString = _configuration.GetConnectionString("DefaultConnection");
            SqlConnection conn = new SqlConnection(connString);
            conn.Open();
            
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SP_Department_GetAll";
            
            SqlDataReader rdr = cmd.ExecuteReader();
            DataTable tbl = new DataTable();
            tbl.Load(rdr);
            conn.Close();
            
            ViewBag.Departments = new SelectList(tbl.Rows.Cast<DataRow>(), "DepartmentID", "DepartmentName", staff.DepartmentId);
            return View(staff);
        }

        public IActionResult Details(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "SP_Staff_GetByID";
            command.Parameters.AddWithValue("@StaffID", id);
            
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            connection.Close();
            
            if (table.Rows.Count == 0)
            {
                return NotFound();
            }
            
            DataRow row = table.Rows[0];
            Staff staff = new Staff
            {
                StaffId = Convert.ToInt32(row["StaffID"]),
                DepartmentId = Convert.ToInt32(row["DepartmentID"]),
                StaffName = row["StaffName"].ToString(),
                MobileNo = row["MobileNo"].ToString(),
                EmailAddress = row["EmailAddress"].ToString(),
                Remarks = row["Remarks"] != DBNull.Value ? row["Remarks"].ToString() : null
            };
            
            ViewBag.DepartmentName = row["DepartmentName"].ToString();
            
            return View(staff);
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
                command.CommandText = "SP_Staff_Delete";
                command.Parameters.AddWithValue("@StaffID", id);
                
                command.ExecuteNonQuery();
                connection.Close();
                
                TempData["Success"] = "Staff Member deleted successfully!";
            }
            catch(SqlException)
            {
                TempData["Error"] = "Cannot delete staff member as they are likely associated with meetings.";
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}
