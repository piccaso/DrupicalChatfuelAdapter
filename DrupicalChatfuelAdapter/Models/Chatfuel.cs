using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrupicalChatfuelAdapter.Models
{
    public class Chatfuel
    {
        public Attachment attachment { get; set; }
    }

    public class Attachment
    {
        public string type { get; set; }
        public Payload payload { get; set; }
    }

    public class Payload
    {
        public string template_type { get; set; }
        public Element[] elements { get; set; }
    }

    public class Element
    {
        public string title { get; set; }
        public string image_url { get; set; }
        public string subtitle { get; set; }
        public Button[] buttons { get; set; }
    }

    public class Button
    {
        public string type { get; set; }
        public string url { get; set; }
        public string title { get; set; }
    }

}