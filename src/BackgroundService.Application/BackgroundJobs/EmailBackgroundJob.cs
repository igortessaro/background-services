using BackgroundService.Application.Services;
using Hangfire;
using Hangfire.Server;

namespace BackgroundService.Application.BackgroundJobs
{
    public class EmailBackgroundJob : IEmailBackgroundJob
    {
        private readonly ILogger<EmailBackgroundJob> _logger;
        private readonly ISendMailService _sendMailService;
        private readonly ISmsBackgroundJob _smsBackgroundJob;
        private readonly Guid _id;

        public EmailBackgroundJob(ILogger<EmailBackgroundJob> logger, ISendMailService sendMailService, ISmsBackgroundJob smsBackgroundJob)
        {
            this._logger = logger;
            this._sendMailService = sendMailService;
            this._smsBackgroundJob = smsBackgroundJob;
            this._id = Guid.NewGuid();
            this._logger.LogWarning($"{nameof(EmailBackgroundJob)} instance {this._id.ToString()}");
        }

        public async Task RunAsync(PerformContext performContext)
        {
            _logger.LogInformation($"EmailBackgroundJob is running. ID {performContext.BackgroundJob.Id} | Created at {performContext.BackgroundJob.CreatedAt}");
            await _sendMailService.SendEmailAsync(to: "teste@teste.com", subject: "Teste", body: "Teste");
            BackgroundJob.ContinueJobWith(performContext.BackgroundJob.Id, () => this._smsBackgroundJob.RunAsync(null));
        }
    }
}
