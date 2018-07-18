using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrawler.Crawler
{
    public class Topic
    {
        public Topic()
        {
            Images = new List<TopicImage>();
        }
        public string TopicName { get; set; }
        public string TopicUrl { get; set; }
        public List<TopicImage> Images { get; set; }
    }
}
