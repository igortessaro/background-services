using BackgroundService.Application.BackgroundJobs;
using BackgroundService.Application.Services;
using Hangfire;
using Hangfire.MemoryStorage;

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
builder.Services.AddScoped<IEmailBackgroundJob, EmailBackgroundJob>();

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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
BackgroundJob.Enqueue(() => emailBackgroundJob.RunAsync(null));

app.MapGet("/", () => "Hello World!");
app.MapGet("/person", () =>
{
    var person = new
    {
        Name = "John Doe",
        Age = 42
    };
    return person;
});

app.Run();
