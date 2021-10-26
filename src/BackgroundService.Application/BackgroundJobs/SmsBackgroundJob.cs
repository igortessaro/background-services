using BackgroundService.Application.Services;
using Hangfire.Server;

namespace BackgroundService.Application.BackgroundJobs
{
    public class SmsBackgroundJob : ISmsBackgroundJob
    {
        private readonly ILogger<SmsBackgroundJob> _logger;
        private readonly ISendSmsService _sendSmsService;
        private readonly Guid _id;

        public SmsBackgroundJob(ILogger<SmsBackgroundJob> logger, ISendSmsService sendSmsService)
        {
            this._logger = logger;
            this._sendSmsService = sendSmsService;
            this._id = Guid.NewGuid();
            this._logger.LogWarning($"{nameof(SmsBackgroundJob)} instance {this._id.ToString()}");
        }

        public async Task RunAsync(PerformContext performContext)
        {
            _logger.LogInformation($"SmsBackgroundJob is running. ID {performContext.BackgroundJob.Id} | Created at {performContext.BackgroundJob.CreatedAt}");
            await _sendSmsService.SendSmsAsync(phoneNumber: "09123456789", message: "Hello World");
        }
        
    }
}