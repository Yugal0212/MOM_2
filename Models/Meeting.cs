using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MOM.Models
{
    [Table("MOM_Meetings")]
    public class Meeting
    {
        [Key]
        public int MeetingId { get; set; }

        [Required]
        public DateTime MeetingDate { get; set; }

        [Required]
        public int MeetingVenueId { get; set; }

        [Required]
        public int MeetingTypeId { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [StringLength(250)]
        public string? MeetingDescription { get; set; }

        [StringLength(250)]
        public string? DocumentPath { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime Modified { get; set; } = DateTime.Now;

        public bool? IsCancelled { get; set; }

        public DateTime? CancellationDateTime { get; set; }

        [StringLength(250)]
        public string? CancellationReason { get; set; }

        // Mapped Properties
        [NotMapped]
        public string Title => MeetingDescription ?? "Meeting"; 
        
        [NotMapped]
        public TimeSpan StartTime { get; set; } // Will need handling in Controller to save to MeetingDate
        [NotMapped]
        public TimeSpan EndTime { get; set; }

        [NotMapped]
        public string? MeetingTypeName { get; set; }
        [NotMapped]
        public string? VenueName { get; set; }
        [NotMapped]
        public string? DepartmentName { get; set; }
        [NotMapped]
        public string Status { get; set; } = "Scheduled";
        [NotMapped]
        public string Priority { get; set; } = "Medium";
    }
}
