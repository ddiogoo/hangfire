using Hangfire;
using HangFire.Web.Jobs;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddHangfire(config =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    config.UseSqlServerStorage(connectionString);
    config.UseColouredConsoleLogProvider();
});
builder.Services.AddHangfireServer();

var app = builder.Build();

app.MapHangfireDashboard("/hangfire");

app.MapGet("/", () => "Hello World!");

app.MapGet("/pull", (IBackgroundJobClient client) =>
{
    var url = "https://consultwithgriff.com/rss.xml";
    var directory = $"c:\\rss";
    var filename = "consultwithgriff.json";
    var tempPath = Path.Combine(directory, filename);

    client.Enqueue<WebPuller>(p => p.GetRssItemUrlAsync(url, tempPath));
});

app.Run();
