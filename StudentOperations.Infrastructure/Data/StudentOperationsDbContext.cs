
using Microsoft.EntityFrameworkCore;
using StudentOperations.Domain.Entities;

namespace StudentOperations.Infrastructure.Data;

public class StudentOperationsDbContext : DbContext
{
    public StudentOperationsDbContext(
        DbContextOptions<StudentOperationsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Student> Students => Set<Student>();

    public DbSet<EnrollmentRequest> EnrollmentRequests =>
        Set<EnrollmentRequest>();

    public DbSet<EnrollmentProcessingLog> EnrollmentProcessingLogs =>
        Set<EnrollmentProcessingLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>()
            .HasIndex(s => s.StudentNumber)
            .IsUnique();

        modelBuilder.Entity<EnrollmentRequest>()
            .HasOne(e => e.Student)
            .WithMany()
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EnrollmentProcessingLog>()
            .HasOne(l => l.EnrollmentRequest)
            .WithMany()
            .HasForeignKey(l => l.EnrollmentRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
