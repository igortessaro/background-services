namespace BackgroundService.Application.Services
{
    public class SendMailService : ISendMailService
    {
        private readonly ILogger<SendMailService> _logger;
        private readonly Guid _id;

        public SendMailService(ILogger<SendMailService> logger)
        {
            _logger = logger;
            this._id = Guid.NewGuid();
            this._logger.LogWarning($"{nameof(SendMailService)} instance {this._id.ToString()}");
        }

        public Task SendEmailAsync(string to, string subject, string body)
        {
            return Task.Run(() =>
            {
                _logger.LogInformation($"Sending email to {to} with subject {subject}");
            });
        }
    }
}
