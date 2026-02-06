using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MOM.Models
{
    [Table("MOM_Staff")]
    public class Staff
    {
        [Key]
        public int StaffId { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(50)]
        public string StaffName { get; set; }

        [Required]
        [StringLength(20)]
        public string MobileNo { get; set; }

        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [StringLength(250)]
        public string? Remarks { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime Modified { get; set; } = DateTime.Now;
        
        // Not Mapped properties for View helpers
        [NotMapped]
        public string FullName => StaffName;
    }
}
