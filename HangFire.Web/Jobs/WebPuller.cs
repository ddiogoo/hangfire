using System.ServiceModel.Syndication;
using System.Text.Json;
using System.Xml;

namespace HangFire.Web.Jobs
{
    public class WebPuller
    {
        private readonly ILogger<WebPuller> _logger;

        public WebPuller(ILogger<WebPuller> logger)
        {
            _logger = logger;
        }

        public async Task GetRssItemUrlAsync(string rssFeedUrl, string filename)
        {
            var directory = Path.GetDirectoryName(filename);
            if(!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var client = new HttpClient();
            var rssContent = await client.GetStringAsync(rssFeedUrl);

            using var xmlReader = XmlReader.Create(new StringReader(rssContent));
            var feed = SyndicationFeed.Load(xmlReader);

            var rssItemUrls = feed.Items.Select(item => item.Links.FirstOrDefault()?.Uri.AbsoluteUri).ToList();
            var json = JsonSerializer.Serialize(rssItemUrls);
            await File.WriteAllTextAsync(filename, json);
        }

        public async Task DownloadFileFromUrl(string url, string filePath)
        {
            using var client = new HttpClient();
            using(_logger.BeginScope($"DownloadFileFromUrl({url}, {filePath})"))
            {
                try
                {
                    _logger.LogInformation($"Downloading file from {url}...");
                    using var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    _logger.LogInformation($"Writing file to {filePath}");
                    await using var stream = await response.Content.ReadAsStreamAsync();
                    await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);

                    await stream.CopyToAsync(fileStream);
                } catch(HttpRequestException e)
                {
                    _logger.LogError(e, $"Error downloading file from {url}");
                }
            }
        }
    }
}
