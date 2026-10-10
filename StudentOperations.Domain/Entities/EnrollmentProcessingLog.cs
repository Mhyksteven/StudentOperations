
namespace StudentOperations.Domain.Entities;

public class EnrollmentProcessingLog
{
    public int Id { get; set; }

    public int EnrollmentRequestId { get; set; }

    public EnrollmentRequest? EnrollmentRequest { get; set; }

    public string Message { get; set; } = string.Empty;

    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;

    public bool IsSuccessful { get; set; }
}
