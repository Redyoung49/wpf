using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrawler.Crawler
{
    public class PicturePage
    {
        public PicturePage()
        {
            Topics = new List<Topic>();
        }
        public string PageUrl { get; set; }
        public List<Topic> Topics { get; set; }
    }
}
