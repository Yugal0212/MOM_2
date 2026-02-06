using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Text.Json;
using System.Collections.Generic;

namespace MOM.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IConfiguration _configuration;

        public DashboardController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                
                // Get all data using existing stored procedures
                // Get Meeting Types
                SqlCommand mtCmd = new SqlCommand("SP_MeetingType_GetAll", conn);
                mtCmd.CommandType = CommandType.StoredProcedure;
                DataTable mtTable = new DataTable();
                mtTable.Load(mtCmd.ExecuteReader());
                int totalMeetingTypes = mtTable.Rows.Count;
                
                // Get Meetings
                SqlCommand meetingCmd = new SqlCommand("SP_Meeting_GetAll", conn);
                meetingCmd.CommandType = CommandType.StoredProcedure;
                DataTable meetingTable = new DataTable();
                meetingTable.Load(meetingCmd.ExecuteReader());
                int totalMeetings = meetingTable.Rows.Count;
                
                // Get Staff
                SqlCommand staffCmd = new SqlCommand("SP_Staff_GetAll", conn);
                staffCmd.CommandType = CommandType.StoredProcedure;
                DataTable staffTable = new DataTable();
                staffTable.Load(staffCmd.ExecuteReader());
                int totalStaff = staffTable.Rows.Count;
                
                // Get Venues
                SqlCommand venueCmd = new SqlCommand("SP_MeetingVenue_GetAll", conn);
                venueCmd.CommandType = CommandType.StoredProcedure;
                DataTable venueTable = new DataTable();
                venueTable.Load(venueCmd.ExecuteReader());
                int totalVenues = venueTable.Rows.Count;
                
                // Get Departments
                SqlCommand deptCmd = new SqlCommand("SP_Department_GetAll", conn);
                deptCmd.CommandType = CommandType.StoredProcedure;
                DataTable deptTable = new DataTable();
                deptTable.Load(deptCmd.ExecuteReader());
                int totalDepartments = deptTable.Rows.Count;
                
                // Calculate meetings this month/week
                var now = DateTime.Now;
                var startOfMonth = new DateTime(now.Year, now.Month, 1);
                var startOfWeek = now.AddDays(-(int)now.DayOfWeek);
                
                int meetingsThisMonth = 0;
                int meetingsThisWeek = 0;
                int meetingsLastMonth = 0;
                
                foreach (DataRow row in meetingTable.Rows)
                {
                    DateTime meetingDate = Convert.ToDateTime(row["MeetingDate"]);
                    if (meetingDate >= startOfMonth)
                        meetingsThisMonth++;
                    if (meetingDate >= startOfWeek)
                        meetingsThisWeek++;
                    if (meetingDate >= startOfMonth.AddMonths(-1) && meetingDate < startOfMonth)
                        meetingsLastMonth++;
                }
                
                // Recent meetings (top 5)
                var recentMeetingsData = new List<object>();
                int count = 0;
                foreach (DataRow row in meetingTable.Rows)
                {
                    if (count >= 5) break;
                    DateTime meetingDate = Convert.ToDateTime(row["MeetingDate"]);
                    recentMeetingsData.Add(new
                    {
                        Type = row["MeetingTypeName"].ToString(),
                        Department = row["DepartmentName"].ToString(),
                        Venue = row["MeetingVenueName"].ToString(),
                        Date = meetingDate.ToString("MMM dd, yyyy"),
                        Status = meetingDate > now ? "Scheduled" : "Completed",
                        StatusClass = meetingDate > now ? "warning" : "success"
                    });
                    count++;
                }
                
                // Upcoming meetings (next 3)
                var upcomingMeetingsData = new List<object>();
                count = 0;
                foreach (DataRow row in meetingTable.Rows)
                {
                    DateTime meetingDate = Convert.ToDateTime(row["MeetingDate"]);
                    if (meetingDate >= now && count < 3)
                    {
                        upcomingMeetingsData.Add(new
                        {
                            Title = !string.IsNullOrEmpty(row["MeetingDescription"].ToString()) 
                                ? row["MeetingDescription"].ToString() 
                                : $"Meeting #{row["MeetingID"]}",
                            DateTime = meetingDate.ToString("MMM dd 'at' h:mm tt"),
                            Attendees = 0,
                            Priority = "Medium"
                        });
                        count++;
                    }
                }
                
                // Monthly statistics (last 12 months)
                var scheduledStats = new List<int>();
                var completedStats = new List<int>();
                var cancelledStats = new List<int>();
                
                for (int i = 0; i < 12; i++)
                {
                    var monthStart = startOfMonth.AddMonths(-11 + i);
                    var monthEnd = monthStart.AddMonths(1);
                    int scheduled = 0, completed = 0, cancelled = 0;
                    
                    foreach (DataRow row in meetingTable.Rows)
                    {
                        DateTime meetingDate = Convert.ToDateTime(row["MeetingDate"]);
                        bool isCancelled = row["IsCancelled"] != DBNull.Value && Convert.ToBoolean(row["IsCancelled"]);
                        
                        if (meetingDate >= monthStart && meetingDate < monthEnd)
                        {
                            if (isCancelled)
                                cancelled++;
                            else if (meetingDate > now)
                                scheduled++;
                            else
                                completed++;
                        }
                    }
                    
                    scheduledStats.Add(scheduled);
                    completedStats.Add(completed);
                    cancelledStats.Add(cancelled);
                }
                
                // Staff distribution by department
                var deptLabels = new List<string>();
                var deptValues = new List<int>();
                
                foreach (DataRow dept in deptTable.Rows)
                {
                    string deptName = dept["DepartmentName"].ToString();
                    int deptId = Convert.ToInt32(dept["DepartmentID"]);
                    int staffCount = 0;
                    
                    foreach (DataRow staff in staffTable.Rows)
                    {
                        if (Convert.ToInt32(staff["DepartmentID"]) == deptId)
                            staffCount++;
                    }
                    
                    deptLabels.Add(deptName);
                    deptValues.Add(staffCount);
                }
                
                conn.Close();
                
                // Calculate percentage change
                var percentageChange = meetingsLastMonth > 0 
                    ? ((meetingsThisMonth - meetingsLastMonth) * 100 / meetingsLastMonth) 
                    : 0;
                
                var dashboardData = new
                {
                    MeetingTypes = new
                    {
                        Total = totalMeetingTypes,
                        NewThisMonth = 0,
                        Active = totalMeetingTypes
                    },
                    TotalMeetings = new
                    {
                        Total = totalMeetings,
                        ThisMonth = meetingsThisMonth,
                        PercentageIncrease = percentageChange,
                        ThisWeek = meetingsThisWeek
                    },
                    ActiveStaff = new
                    {
                        Total = totalStaff,
                        AttendanceRate = 94,
                        Active = totalStaff
                    },
                    Venues = new
                    {
                        Total = totalVenues,
                        Utilization = 78,
                        Available = totalVenues
                    },
                    Departments = new
                    {
                        Total = totalDepartments
                    },
                    RecentMeetings = recentMeetingsData,
                    MonthlyStatistics = new
                    {
                        Scheduled = scheduledStats.ToArray(),
                        Completed = completedStats.ToArray(),
                        Cancelled = cancelledStats.ToArray()
                    },
                    StaffDistribution = new
                    {
                        Values = deptValues.ToArray(),
                        Labels = deptLabels.ToArray()
                    },
                    RecentActivity = new[]
                    {
                        new { Time = "Recent", Type = "success", Message = $"Total of {totalMeetings} meetings in system" },
                        new { Time = "This Month", Type = "primary", Message = $"{meetingsThisMonth} meetings scheduled this month" },
                        new { Time = "This Week", Type = "info", Message = $"{meetingsThisWeek} meetings this week" },
                        new { Time = "Active", Type = "warning", Message = $"{totalStaff} staff members registered" },
                        new { Time = "System", Type = "success", Message = $"{totalVenues} venues available for booking" }
                    },
                    UpcomingMeetings = upcomingMeetingsData
                };

                ViewBag.DashboardData = JsonSerializer.Serialize(dashboardData);
                return View(dashboardData);
            }
        }
    }
}