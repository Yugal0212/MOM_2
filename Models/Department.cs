using System;
using System.ComponentModel.DataAnnotations;

namespace MOM.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Department Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Department Code")]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Manager")]
        public string Manager { get; set; } = string.Empty;

        [Display(Name = "Employee Count")]
        public int EmployeeCount { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;
    }
}
