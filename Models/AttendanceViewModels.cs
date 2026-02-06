using System;
using System.Collections.Generic;

namespace MOM.Models
{
    public class AttendanceViewModel
    {
        public int MeetingId { get; set; }
        public string MeetingTitle { get; set; }
        public DateTime MeetingDate { get; set; }
        public List<StaffAttendanceItem> StaffList { get; set; } = new List<StaffAttendanceItem>();
    }

    public class StaffAttendanceItem
    {
        public int StaffId { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public bool IsPresent { get; set; }
        public string Remarks { get; set; }
    }

    public class AttendanceSummaryViewModel
    {
        public Meeting Meeting { get; set; }
        public int TotalStaff { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public bool IsAttendanceTaken { get; set; }
    }
}
