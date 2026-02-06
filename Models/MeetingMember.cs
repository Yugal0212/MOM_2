using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MOM.Models
{
    [Table("MOM_MeetingMember")]
    public class MeetingMember
    {
        [Key]
        public int MeetingMemberId { get; set; }

        [Required]
        public int MeetingId { get; set; }

        [Required]
        public int StaffId { get; set; }

        [Required]
        public bool IsPresent { get; set; }

        [StringLength(250)]
        public string? Remarks { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime Modified { get; set; } = DateTime.Now;

        // Display properties
        [NotMapped]
        public string? StaffName { get; set; }
        [NotMapped]
        public string? MeetingTitle { get; set; }
    }
}
