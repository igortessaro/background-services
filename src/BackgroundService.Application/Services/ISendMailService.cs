namespace BackgroundService.Application.Services
{
    public interface ISendMailService
    {
         Task SendEmailAsync(string to, string subject, string body);
    }
}