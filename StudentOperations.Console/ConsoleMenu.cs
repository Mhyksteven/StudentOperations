
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using StudentOperations.Application;

namespace StudentOperations.ConsoleApp;

public class ConsoleMenu
{
    private readonly IEnrollmentService _service;
    private readonly IServiceScopeFactory _scopeFactory;

    public ConsoleMenu(
        IEnrollmentService service,
        IServiceScopeFactory scopeFactory)
    {
        _service = service;
        _scopeFactory = scopeFactory;
    }

    public async Task RunAsync()
    {
        while (true)
        {
            Console.WriteLine("\nSTUDENT OPERATIONS");
            Console.WriteLine("1. Register student");
            Console.WriteLine("2. Create enrollment request");
            Console.WriteLine("3. Immediate job");
            Console.WriteLine("4. Delayed job");
            Console.WriteLine("5. Continuation job");
            Console.WriteLine("6. Batch job");
            Console.WriteLine("7. Show processing history");
            Console.WriteLine("0. Exit");
            Console.Write("Select: ");

            switch (Console.ReadLine())
            {
                case "1":
                    Console.Write("Full name: ");
                    var name = Console.ReadLine() ?? "";
                    Console.Write("Email: ");
                    var email = Console.ReadLine() ?? "";
                    Console.WriteLine(
                        $"Student ID: {await _service.RegisterStudentAsync(name, email)}");
                    break;

                case "2":
                    Console.Write("Student ID: ");
                    if (int.TryParse(Console.ReadLine(), out var studentId))
                        Console.WriteLine(
                            $"Request ID: {await _service.CreateEnrollmentRequestAsync(studentId)}");
                    else
                        Console.WriteLine("Invalid student ID.");
                    break;

                case "3":
                    Console.Write("Request ID: ");
                    if (int.TryParse(Console.ReadLine(), out var immediateId))
                        BackgroundJob.Enqueue<EnrollmentJob>(
                            job => job.ProcessAsync(immediateId));
                    break;

                case "4":
                    Console.Write("Request ID: ");
                    if (int.TryParse(Console.ReadLine(), out var delayedId))
                        BackgroundJob.Schedule<EnrollmentJob>(
                            job => job.ProcessAsync(delayedId),
                            TimeSpan.FromMinutes(1));
                    break;

                case "5":
                    Console.Write("Request ID: ");
                    if (int.TryParse(Console.ReadLine(), out var parentId))
                    {
                        var jobId = BackgroundJob.Enqueue<EnrollmentJob>(
                            job => job.ProcessAsync(parentId));

                        BackgroundJob.ContinueJobWith<EnrollmentJob>(
                            jobId, job => job.SendConfirmationAsync(parentId));
                    }
                    break;

                case "6":
                    BackgroundJob.Enqueue<EnrollmentJob>(
                        job => job.ProcessBatchAsync());
                    Console.WriteLine("Batch job queued.");
                    break;

                case "7":
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var service = scope.ServiceProvider
                            .GetRequiredService<IEnrollmentService>();
                        var logs = await service.GetLogsAsync();

                        foreach (var log in logs)
                            Console.WriteLine(
                                $"{log.ProcessedAt:u} | {log.Message} | Success: {log.IsSuccessful}");

                        if (logs.Count == 0)
                            Console.WriteLine("No processing history found.");
                    }
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }
}
