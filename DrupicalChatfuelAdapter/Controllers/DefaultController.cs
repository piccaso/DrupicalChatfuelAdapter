using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DrupicalChatfuelAdapter.Models;
using Newtonsoft.Json;

namespace DrupicalChatfuelAdapter.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        public ActionResult Index(int? limit= 10, IEnumerable<string> type = null, IEnumerable<string> city = null, IEnumerable<string> country = null)
        {
            List<Drupical> drupical = null;
            using (var wc = new WebClient())
            {
                var content = wc.DownloadString("https://www.drupical.com/app");
                drupical = JsonConvert.DeserializeObject<List<Drupical>>(content);
            }

            drupical = drupical.OrderBy(d => d.from).ToList();
            if (type != null && type.Any()) drupical = drupical.Where(d => type.Contains(d.type)).ToList();
            if (city != null && city.Any()) drupical = drupical.Where(d => city.Contains(d.city)).ToList();
            if (country != null && country.Any()) drupical = drupical.Where(d => country.Contains(d.country)).ToList();
            if (limit.HasValue && limit.Value > -1) drupical = drupical.Take(limit.Value).ToList();

            var chatfuel = new List<Chatfuel>();

            foreach (var d in drupical)
            {
                var subtitle = new List<string>();

                if(!string.IsNullOrEmpty(d.country)) subtitle.Add(d.country);
                if(!string.IsNullOrEmpty(d.city)) subtitle.Add(d.city);
                //var date = DateTimeOffset.FromUnixTimeSeconds



                var entry = new Chatfuel
                {
                    attachment = new Attachment
                    {
                        type = "template",
                        payload = new Payload
                        {
                            template_type = "generic",
                            elements = new[]
                            {
                                new Element
                                {
                                    title = d.title,
                                    image_url = d.logo,
                                    subtitle = "2do",
                                    buttons = new[]
                                    {
                                        new Button()
                                        {
                                            type = "web_url",
                                            url = d.link,
                                            title = "View Details"
                                        }
                                    }
                                },
                            },
                        }
                    }
                };
                chatfuel.Add(entry);
            }
            

            return Content(JsonConvert.SerializeObject(chatfuel, Formatting.Indented), "application/javascript", Encoding.UTF8);
        }
    }
}