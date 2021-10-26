using Hangfire.Server;

namespace BackgroundService.Application.BackgroundJobs
{
    public interface IBackgroundJob
    {
        Task RunAsync(PerformContext performContext);
    }
}