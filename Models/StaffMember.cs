using System;
using System.ComponentModel.DataAnnotations;

namespace MOM.Models
{
    public class StaffMember
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Department")]
        public string Department { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Position")]
        public string Position { get; set; } = string.Empty;

        [Display(Name = "Employee ID")]
        public string EmployeeId { get; set; } = string.Empty;

        [Display(Name = "Joined Date")]
        [DataType(DataType.Date)]
        public DateTime JoinedDate { get; set; } = DateTime.Now;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        public string FullName => $"{FirstName} {LastName}";
    }
}
