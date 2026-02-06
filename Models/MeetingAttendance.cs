using System;
using System.ComponentModel.DataAnnotations;

namespace MOM.Models
{
    public class MeetingAttendance
    {
        [Key]
        public int MeetingAttendanceId { get; set; }

        [Required]
        public int MeetingId { get; set; }

        [Required]
        public int StaffId { get; set; }

        public bool IsPresent { get; set; }

        public string Remarks { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime Modified { get; set; } = DateTime.Now;

        // Navigation properties (optional for mock, useful for View)
        public string StaffName { get; set; }
        public string Department { get; set; }
        public string MeetingTitle { get; set; }
    }
}
