namespace BackgroundService.Application.Services
{
    public interface ISendSmsService
    {
        Task SendSmsAsync(string phoneNumber, string message);
    }
}