using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MOM.Models
{
    [Table("MOM_MeetingType")]
    public class MeetingType
    {
        [Key]
        public int MeetingTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public string MeetingTypeName { get; set; }

        [Required]
        [StringLength(100)]
        public string Remarks { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime Modified { get; set; } = DateTime.Now;
    }
}
