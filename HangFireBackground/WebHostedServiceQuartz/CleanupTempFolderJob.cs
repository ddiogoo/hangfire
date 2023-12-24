using Quartz;

namespace WebHostedServiceQuartz;

public class CleanupTempFolderJob : IJob
{
    private ILogger<CleanupTempFolderJob> _logger;
    private string _tempDirectory;

    public CleanupTempFolderJob(ILogger<CleanupTempFolderJob> logger)
    {
        _logger = logger;

        var tempDirectory = Path.GetTempPath();
        _tempDirectory = Path.Combine(tempDirectory, "CleanUpTempFolderService");

        _logger.LogInformation($"Temp directory is {tempDirectory}");
    }

    public Task Execute(IJobExecutionContext context)
    {
        if (!Directory.Exists(_tempDirectory))
        {
            Directory.CreateDirectory(_tempDirectory);
        }
        _logger.LogInformation($"Checking folder: {_tempDirectory}");

        var files = Directory
            .GetFiles(_tempDirectory, "*.*", SearchOption.AllDirectories)
            .Where(f => File.GetLastWriteTime(f) < DateTime.Now.AddSeconds(-5));

        if (files.Any())
        {
            _logger.LogInformation($"Found {files.Count()} files to delete");
            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                    _logger.LogInformation($"Deleting file {file}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to delete file {file}");
                }
            }
        }

        _logger.LogInformation("Shutgiting down worker");
        return Task.FromResult(true);
    }
}
