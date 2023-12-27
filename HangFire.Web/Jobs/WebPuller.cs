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

    }
}
