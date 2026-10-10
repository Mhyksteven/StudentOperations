
namespace StudentOperations.Application;

public class EnrollmentJob
{
    private readonly IEnrollmentService _service;

    public EnrollmentJob(IEnrollmentService service)
    {
        _service = service;
    }

    public Task ProcessAsync(int requestId)
        => _service.ProcessEnrollmentAsync(requestId);

    public Task ProcessPendingAsync()
        => _service.ProcessPendingAsync();

    public Task ProcessBatchAsync()
        => _service.ProcessBatchAsync();

    public Task SendConfirmationAsync(int requestId)
        => _service.SendConfirmationAsync(requestId);
}
