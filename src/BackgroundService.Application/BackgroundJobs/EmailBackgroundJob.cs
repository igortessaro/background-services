using BackgroundService.Application.Services;
using Hangfire.Server;

namespace BackgroundService.Application.BackgroundJobs
{
    public class EmailBackgroundJob : IEmailBackgroundJob
    {
        private readonly ILogger<EmailBackgroundJob> _logger;
        private readonly ISendMailService _sendMailService;

        public EmailBackgroundJob(ILogger<EmailBackgroundJob> logger, ISendMailService sendMailService)
        {
            _logger = logger;
            _sendMailService = sendMailService;
        }

        public async Task RunAsync(PerformContext performContext)
        {
            _logger.LogInformation($"EmailBackgroundJob is running. ID {performContext.BackgroundJob.Id} | Created at {performContext.BackgroundJob.CreatedAt}");
            await _sendMailService.SendEmailAsync(to: "teste@teste.com", subject: "Teste", body: "Teste");
        }
    }
}
