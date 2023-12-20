using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using System.Globalization;
using System.Text;

namespace IPFees.Web.Pages
{
    public class SitemapModel : PageModel
    {
        private readonly List<SitemapItem> _sitemapItems = new List<SitemapItem>
        {
            new SitemapItem("/Run", changefreq: SitemapChangeFrequency.Monthly, priority: 1.0),            
            // TODO: Add more sitemap items
        };
        private readonly IHttpContextAccessor httpContextAccessor;
        private string RootURL;

        public SitemapModel(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public IActionResult OnGet()
        {
            RootURL = GetRootURL();
            Response.ContentType = "text/xml";
            return new ContentResult
            {
                Content = GenerateSitemapXml(_sitemapItems),
                StatusCode = 200,
            };
        }

        private string GenerateSitemapXml(List<SitemapItem> items)
        {
            var xml = new System.Xml.XmlDocument();
            var root = xml.CreateElement("urlset");
            root.SetAttribute("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");
            root.SetAttribute("xmlns:xhtml", "http://www.w3.org/1999/xhtml");

            foreach (var item in items)
            {
                var url = xml.CreateElement("url", string.Empty);
                var loc = xml.CreateElement("loc");
                loc.InnerText = RootURL + item.Location;
                url.AppendChild(loc);

                var lastmod = xml.CreateElement("lastmod");
                lastmod.InnerText = item.LastModified.ToString("yyyy-MM-ddTHH:mm:ssZ");
                url.AppendChild(lastmod);

                var changefreq = xml.CreateElement("changefreq");
                changefreq.InnerText = item.ChangeFrequency.ToString().ToLower();
                url.AppendChild(changefreq);

                var priority = xml.CreateElement("priority");
                priority.InnerText = item.Priority.ToString("F1", CultureInfo.InvariantCulture);
                url.AppendChild(priority);

                root.AppendChild(url);
            }

            xml.AppendChild(root);
            return xml.InnerXml;
        }

        private string GetRootURL()
        {
            var Request = httpContextAccessor.HttpContext?.Request;
            if (Request != null)
            {
                var Protocol = Request.IsHttps ? "https://" : "http://";
                var Host = Request.Host.ToString();
                return $"{Protocol}{Host}";
            }
            else
            {
                return "Unavailable";
            }
        }
    }

    public class SitemapItem
    {
        public string Location { get; }
        public DateTime LastModified { get; }
        public SitemapChangeFrequency ChangeFrequency { get; }
        public double Priority { get; }

        public SitemapItem(string location, DateTime lastModified = default, SitemapChangeFrequency changefreq = SitemapChangeFrequency.Weekly, double priority = 0.5)
        {
            Location = location;
            LastModified = lastModified == default ? DateTime.UtcNow : lastModified;
            ChangeFrequency = changefreq;
            Priority = priority;
        }
    }

    public enum SitemapChangeFrequency
    {
        Always,
        Hourly,
        Daily,
        Weekly,
        Monthly,
        Yearly,
        Never
    }
}

