using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DrupicalChatfuelAdapter.Models;
using DrupicalChatfuelAdapter.Services;
using Newtonsoft.Json;

namespace DrupicalChatfuelAdapter.Controllers
{
    public class CaseInsensitiveComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return string.Compare(x, y, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        public int GetHashCode(string obj)
        {
            return obj.ToLowerInvariant().GetHashCode();
        }
    }

    public class DefaultController : Controller
    {

        private ICacheService _cache = new InMemoryCache();

        // GET: Default
        public ActionResult Index(int? limit= 10, IEnumerable<string> type = null, IEnumerable<string> city = null, IEnumerable<string> country = null, string format = null)
        {
            var url = "https://www.drupical.com/app";
            var defaultLogo = "https://www.dropbox.com/s/l75ltlq9vy8i7iq/drupical-cover.png?dl=1";
            var drupical = _cache.GetOrSet(url, 60, () =>
            {
                using (var wc = new WebClient())
                {
                    var content = wc.DownloadString(url);
                    return JsonConvert.DeserializeObject<List<Drupical>>(content);
                }
            });
            var comparer = new CaseInsensitiveComparer();
            drupical = drupical.OrderBy(d => d.from).ToList();
            if (type != null && type.Any()) drupical = drupical.Where(d => type.Contains(d.type, comparer)).ToList();
            if (city != null && city.Any()) drupical = drupical.Where(d => city.Contains(d.city, comparer)).ToList();
            if (country != null && country.Any()) drupical = drupical.Where(d => country.Contains(d.country, comparer)).ToList();
            if (limit.HasValue && limit.Value > -1) drupical = drupical.Take(limit.Value).ToList();

            var chatfuel = new List<Chatfuel>();
            var chatfuelElements = new List<Element>();

            foreach (var d in drupical)
            {
                var subtitle = new List<string>();

                if(!string.IsNullOrWhiteSpace(d.country)) subtitle.Add(d.country);
                if(!string.IsNullOrWhiteSpace(d.city)) subtitle.Add(d.city);
                var from = _UnixTimeStampToDateTime(d.from);
                subtitle.Add(from.ToString("dd. MMMM yyyy"));
                if (string.IsNullOrWhiteSpace(d.logo)) d.logo = defaultLogo;
                chatfuelElements.Add(new Element
                {
                    title = d.title,
                    image_url = d.logo ?? defaultLogo,
                    subtitle = string.Join(", ", subtitle),
                    buttons = new[]
                    {
                        new Button()
                        {
                            type = "web_url",
                            url = d.link,
                            title = "View Details"
                        }
                    }
                });
            }

            chatfuel.Add(new Chatfuel
            {
                attachment = new Attachment
                {
                    type = "template",
                    payload = new Payload
                    {
                        template_type = "generic",
                        elements = chatfuelElements.ToArray(),
                    }
                }
            });
            

            string returnContent;
            if (format == null || format == "chatfuel")
            {
                returnContent = JsonConvert.SerializeObject(chatfuel, Formatting.Indented);
            }
            else if(format == "drupical")
            {
                returnContent = JsonConvert.SerializeObject(drupical, Formatting.Indented);
            }
            else
            {
                throw new InvalidOperationException($"Unknown format:'{format}'");
            }

            //returnContent = JsonConvert.SerializeObject(drupical.Select(d => d.country).Distinct().ToArray(), Formatting.Indented);

            return Content(returnContent, "application/javascript", Encoding.UTF8);
        }

        public static DateTime _UnixTimeStampToDateTime(long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}