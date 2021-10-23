using Hangfire.Server;

namespace BackgroundService.Application.BackgroundJobs
{
    public interface IEmailBackgroundJob
    {
         Task RunAsync(PerformContext performContext);
    }
}