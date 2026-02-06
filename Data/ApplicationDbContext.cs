using Microsoft.EntityFrameworkCore;
using MOM.Models;

namespace MOM.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<MeetingType> MeetingTypes { get; set; }
        public DbSet<MeetingVenue> Venues { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<MeetingMember> MeetingMembers { get; set; }
    }
}
