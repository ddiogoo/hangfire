using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Tatics.WindowsServices
{
    public class CleanupFolderService(ILogger<CleanupFolderService> logger) : BackgroundService
    {
        private const string FolderToCheck = @"C:\Temp";
        private const int DelayBetweenChecks = 1000 * 15; // 15 seconds
        private readonly ILogger<CleanupFolderService> _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!Directory.Exists(FolderToCheck))
            {
                Directory.CreateDirectory(FolderToCheck);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Checking folder: {FolderToCheck}");

                var files = Directory
                    .GetFiles(FolderToCheck, "*.*", SearchOption.AllDirectories)
                    .Where(f => File.GetLastWriteTime(f) < DateTime.Now.AddSeconds(-5));

                if (files.Any())
                {
                    _logger.LogInformation($"Found {files.Count()} files to delete");
                }

                foreach (var file in files)
                {
                    _logger.LogInformation($"Deleting file {file}");
                    File.Delete(file);
                }

                await Task.Delay(DelayBetweenChecks, stoppingToken);
            }
            _logger.LogInformation("Shutting down worker");
        }
    }
}
