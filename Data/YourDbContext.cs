using AngularSQLlink.Data;
using Microsoft.EntityFrameworkCore;

namespace AngularSQLlink.Data
{
    public class YourDbContext : DbContext
    {
        public YourDbContext(DbContextOptions<YourDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<LeaveRecord> LeaveRecords { get; set; }
    }
}
