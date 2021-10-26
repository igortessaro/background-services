namespace BackgroundService.Application.Models
{
    public sealed class JobModel
    {
        public string Name { get; set; }
        public string Cron { get; set; }
    }
}