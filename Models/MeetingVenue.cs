using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MOM.Models
{
    [Table("MOM_MeetingVenue")]
    public class MeetingVenue
    {
        [Key]
        public int MeetingVenueId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Venue Name")]
        public string MeetingVenueName { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime Modified { get; set; } = DateTime.Now;
        
        // Helper property for simplified access if needed in views, but we will use the DB names primarily to avoid confusion
        [NotMapped]
        public string Name => MeetingVenueName;
    }
}
