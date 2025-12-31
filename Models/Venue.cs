using System;
using System.ComponentModel.DataAnnotations;

namespace MOM.Models
{
    public class Venue
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Venue Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Location/Address")]
        public string Location { get; set; } = string.Empty;

        [Display(Name = "Building")]
        public string Building { get; set; } = string.Empty;

        [Display(Name = "Room Number")]
        public string RoomNumber { get; set; } = string.Empty;

        [Display(Name = "Capacity")]
        public int Capacity { get; set; } = 0;

        [Required]
        [Display(Name = "Venue Type")]
        public string VenueType { get; set; } = "Physical"; // Physical or Virtual

        [Display(Name = "Meeting Link")]
        public string MeetingLink { get; set; } = string.Empty;

        [Display(Name = "Equipment Available")]
        public string Equipment { get; set; } = string.Empty;

        [Display(Name = "Created Date")]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;
    }
}
