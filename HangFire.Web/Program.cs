using Hangfire;
using HangFire.Web;
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

var url = "https://consultwithgriff.com/rss.xml";
var directory = $"c:\\rss";
var filename = "consultwithgriff.json";
var tempPath = Path.Combine(directory, filename);

RecurringJob.AddOrUpdate<WebPuller>(
    "pull-rss-feed", 
    p => p.GetRssItemUrlAsync(url, tempPath), 
    "* * * * *");

RecurringJob.RemoveIfExists("pull-rss-feed");

app.MapGet("/pull", () =>
{
    RecurringJob.TriggerJob("pull-rss-feed");
});

app.MapGet("/sync", (IBackgroundJobClient client) =>
{
    var directory = $"c:\\rss";
    var filename = "consultwithgriff.json";

    var path = Path.Combine(directory, filename);
    var json = File.ReadAllText(path);
    var rssItemUrls = JsonSerializer.Deserialize<List<string>>(json);

    var outputPath = Path.Combine(directory, "output");
    if(!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);

    if (rssItemUrls == null || rssItemUrls.Count == 0) return;
    var delayInSeconds = 5;

    foreach(var url in rssItemUrls)
    {
        var u = new Uri(url);
        var stub = u.Segments.Last();
        if (stub.EndsWith('/')) stub = stub.Substring(0, stub.Length - 1);
        stub += ".html";

        var filePath = Path.Combine(outputPath, stub);
        client.Schedule<WebPuller>(p => p.DownloadFileFromUrl(url, filePath), TimeSpan.FromSeconds(delayInSeconds));
        delayInSeconds += 5;
    }
});

app.Run();
