using BackgroundService.Application.BackgroundJobs;
using BackgroundService.Application.Models;
using BackgroundService.Application.Services;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Background Services", Version = "v1" });
});

builder.Services.AddHangfire(op =>
{
    op.UseMemoryStorage();
});

builder.Services.AddHangfireServer();

builder.Services.AddScoped<ISendMailService, SendMailService>();
builder.Services.AddScoped<ISendSmsService, SendSmsService>();
builder.Services.AddScoped<IEmailBackgroundJob, EmailBackgroundJob>();
builder.Services.AddScoped<ISmsBackgroundJob, SmsBackgroundJob>();

using var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Background Services v1");
        c.RoutePrefix = string.Empty;
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.UseHangfireDashboard();
var serviceScopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using var scope = serviceScopeFactory.CreateScope();

var emailBackgroundJob = scope.ServiceProvider.GetRequiredService<IEmailBackgroundJob>();
var smsBackgroundJob = scope.ServiceProvider.GetRequiredService<ISmsBackgroundJob>();

string enqueueId = BackgroundJob.Enqueue(() => emailBackgroundJob.RunAsync(null));
string scheduleJobId = BackgroundJob.Schedule(() => emailBackgroundJob.RunAsync(null), TimeSpan.FromMinutes(5));
RecurringJob.AddOrUpdate(() => emailBackgroundJob.RunAsync(null), Cron.Minutely);

app.MapPost("/backgroundjob/recurring", (IServiceProvider serviceProvider, JobModel job) =>
{
    var type = typeof(IBackgroundJob);
    var backgroundJob = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(s => s.GetTypes())
        .Where(p => type.IsAssignableFrom(p) && p.IsInterface && p.Name.StartsWith($"I{job.Name}"))
        .FirstOrDefault();

    if (backgroundJob is null)
    {
        return Results.BadRequest($"{job.Name} job is not registered");
    }

    IBackgroundJob? backgroundFromAdd = serviceProvider.GetService(backgroundJob) as IBackgroundJob;

    if (backgroundFromAdd is null)
    {
        return Results.BadRequest($"{job.Name} job is not registered");
    }

    RecurringJob.AddOrUpdate(() => backgroundFromAdd.RunAsync(null), job.Cron);

    return Results.Ok("Enqueued");
});
app.MapPost("/backgroundjob/enqueue/{jobName}", (IServiceProvider serviceProvider, string jobName) =>
{
    var type = typeof(IBackgroundJob);
    var backgroundJob = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(s => s.GetTypes())
        .Where(p => type.IsAssignableFrom(p) && p.IsInterface && p.Name.StartsWith($"I{jobName}"))
        .FirstOrDefault();

    if (backgroundJob is null)
    {
        return Results.BadRequest($"{jobName} job is not registered");
    }

    IBackgroundJob? backgroundJobFromEnqueue = serviceProvider.GetService(backgroundJob) as IBackgroundJob;

    if (backgroundJobFromEnqueue is null)
    {
        return Results.BadRequest($"{jobName} job is not registered");
    }

    BackgroundJob.Enqueue(() => backgroundJobFromEnqueue.RunAsync(null));

    return Results.Ok("Enqueued");
});
app.MapGet("/backgroundjob", () =>
{
    var type = typeof(IBackgroundJob);
    var types = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(s => s.GetTypes())
        .Where(p => type.IsAssignableFrom(p) && p.IsInterface && !p.Name.Equals(nameof(IBackgroundJob)))
        .Select(x => x.Name);

    return types;
});

app.Run();
