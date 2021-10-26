namespace BackgroundService.Application.Services
{
    public class SendSmsService : ISendSmsService
    {
        private readonly ILogger<SendSmsService> _logger;

        public SendSmsService(ILogger<SendSmsService> logger)
        {
            _logger = logger;
        }

        public Task SendSmsAsync(string phoneNumber, string message)
        {
            return Task.Run(() =>
            {
                _logger.LogInformation($"Sending sms to {phoneNumber} with message {message}");
            });
        }
    }
}