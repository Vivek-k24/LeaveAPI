using Microsoft.EntityFrameworkCore;
using AngularSQLlink.Models;

namespace AngularSQLlink.Data
{
    public class YourDbContext : DbContext
    {
        public YourDbContext(DbContextOptions<YourDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<LeaveRecord> LeaveRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmployeeId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Location).IsRequired().HasMaxLength(100);
                entity.Property(e => e.JobTitle).IsRequired().HasMaxLength(100);
                entity.Property(e => e.SubUnit).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<LeaveRecord>(entity =>
            {
                entity.HasKey(lr => lr.LeaveRecordId);
                entity.Property(lr => lr.LeaveType).IsRequired().HasMaxLength(50);
                entity.Property(lr => lr.Entitlements).IsRequired();
                entity.Property(lr => lr.PendingApproval).IsRequired();
                entity.Property(lr => lr.Scheduled).IsRequired();
                entity.Property(lr => lr.Taken).IsRequired();
                entity.Property(lr => lr.Balance).IsRequired();
                entity.Property(lr => lr.LeavePeriod).IsRequired();

                entity.HasOne<Employee>()
                    .WithMany()
                    .HasForeignKey(lr => lr.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed data
            modelBuilder.Entity<Employee>().HasData(
                new Employee { EmployeeId = 1, Name = "Rajkumar", Location = "New York Sales Office", JobTitle = "Sales Representative", SubUnit = "Sales" }
            );

        }
    }
}
