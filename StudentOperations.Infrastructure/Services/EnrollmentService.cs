
using Microsoft.EntityFrameworkCore;
using StudentOperations.Application;
using StudentOperations.Domain.Entities;
using StudentOperations.Infrastructure.Data;

namespace StudentOperations.Infrastructure.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly StudentOperationsDbContext _db;

    public EnrollmentService(StudentOperationsDbContext db)
    {
        _db = db;
    }

    public async Task<int> RegisterStudentAsync(
        string fullName, string email)
    {
        var parts = fullName.Trim().Split(
            ' ', 2, StringSplitOptions.RemoveEmptyEntries);

        var student = new Student
        {
            StudentNumber = $"STU-{Guid.NewGuid():N}"[..12].ToUpperInvariant(),
            FirstName = parts.Length > 0 ? parts[0] : "",
            LastName = parts.Length > 1 ? parts[1] : "",
            Email = email.Trim()
        };

        _db.Students.Add(student);
        await _db.SaveChangesAsync();

        return student.Id;
    }

    public async Task<int> CreateEnrollmentRequestAsync(
        int studentId, bool simulateTransientFailure = false)
    {
        var studentExists = await _db.Students
            .AnyAsync(s => s.Id == studentId);

        if (!studentExists)
            throw new InvalidOperationException("Student not found.");

        var request = new EnrollmentRequest
        {
            StudentId = studentId,
            CourseCode = "GENERAL",
            Status = EnrollmentStatus.Pending,
            Remarks = simulateTransientFailure
                ? "Transient failure simulation requested"
                : null
        };

        _db.EnrollmentRequests.Add(request);
        await _db.SaveChangesAsync();

        return request.Id;
    }

    public async Task ProcessEnrollmentAsync(int requestId)
    {
        var request = await _db.EnrollmentRequests
            .FirstOrDefaultAsync(r => r.Id == requestId);

        if (request is null)
            throw new InvalidOperationException("Enrollment request not found.");

        request.Status = EnrollmentStatus.Approved;

        _db.EnrollmentProcessingLogs.Add(new EnrollmentProcessingLog
        {
            EnrollmentRequestId = request.Id,
            Message = $"Enrollment request {request.Id} approved.",
            IsSuccessful = true
        });

        await _db.SaveChangesAsync();
    }

    public async Task ProcessPendingAsync()
    {
        var pendingIds = await _db.EnrollmentRequests
            .Where(r => r.Status == EnrollmentStatus.Pending)
            .Select(r => r.Id)
            .ToListAsync();

        foreach (var id in pendingIds)
            await ProcessEnrollmentAsync(id);
    }

    public async Task ProcessBatchAsync()
    {
        await ProcessPendingAsync();
    }

    public async Task SendConfirmationAsync(int requestId)
    {
        var exists = await _db.EnrollmentRequests
            .AnyAsync(r => r.Id == requestId);

        if (!exists)
            throw new InvalidOperationException("Enrollment request not found.");

        _db.EnrollmentProcessingLogs.Add(new EnrollmentProcessingLog
        {
            EnrollmentRequestId = requestId,
            Message = $"Confirmation recorded for request {requestId}.",
            IsSuccessful = true
        });

        await _db.SaveChangesAsync();
    }

    public Task<List<EnrollmentProcessingLog>> GetLogsAsync()
    {
        return _db.EnrollmentProcessingLogs
            .OrderByDescending(l => l.ProcessedAt)
            .ToListAsync();
    }
}
