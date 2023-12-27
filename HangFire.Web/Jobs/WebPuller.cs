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

    }
}
