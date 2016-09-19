using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrupicalChatfuelAdapter.Models
{
    public class Drupical
    {
        public string nid { get; set; }
        public string title { get; set; }
        public long from { get; set; }
        //public Date date { get; set; }
        public Geo geo { get; set; }
        public object[] map { get; set; }
        public string country { get; set; }
        public string continent { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string gid { get; set; }
        public long to { get; set; }
        public string source { get; set; }
        public string link { get; set; }
        public string logo { get; set; }
        public string type { get; set; }
        public string venue { get; set; }
        public string thoroughfare { get; set; }
        public string postal_code { get; set; }
    }

    public class Date
    {
        public long from { get; set; }
        public long to { get; set; }
    }

    public class Geo
    {
        public string lat { get; set; }
        public string lon { get; set; }
    }

}