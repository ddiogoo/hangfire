using Quartz;
using WebHostedServiceQuartz;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(configure => configure.AddConsole());
builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("CleanupTempFolderJob");
    q.AddJob<CleanupTempFolderJob>(jobKey, job => job.StoreDurably()
        .DisallowConcurrentExecution()
        .WithDescription("Cleans up the temp folder"));

    var cronEvery15Seconds = "0 / 15 * * ? * *";
    q.AddTrigger(trigger => trigger.ForJob((jobKey))
        .WithDescription(cronEvery15Seconds));
});
builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
