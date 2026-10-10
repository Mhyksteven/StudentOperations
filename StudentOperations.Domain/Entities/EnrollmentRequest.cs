
namespace StudentOperations.Domain.Entities;

public class EnrollmentRequest
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public Student? Student { get; set; }

    public string CourseCode { get; set; } = string.Empty;

    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Pending;

    public string? Remarks { get; set; }
}
