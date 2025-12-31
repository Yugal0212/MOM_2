using System;
using System.ComponentModel.DataAnnotations;

namespace MOM.Models
{
    public class MeetingType
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Type Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Type Code")]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Default Duration (minutes)")]
        public int DefaultDuration { get; set; } = 60;

        [Display(Name = "Created Date")]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;
    }
}
