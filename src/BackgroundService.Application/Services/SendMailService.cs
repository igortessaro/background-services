namespace BackgroundService.Application.Services
{
    public class SendMailService : ISendMailService
    {
        private readonly ILogger<SendMailService> _logger;

        public SendMailService(ILogger<SendMailService> logger)
        {
            _logger = logger;
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
