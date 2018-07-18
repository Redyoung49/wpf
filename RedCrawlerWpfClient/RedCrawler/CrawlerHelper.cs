using HtmlAgilityPack;
using RedCrawlerWpfClient.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrawlerWpfClient.RedCrawler
{
    public class CrawlerHelper
    {
        public ILogger Logger => DefaultLogger.Instance;
        public List<TopicViewModel> CrawlPage(string pageUrl)
        {
            List<TopicViewModel> topics = new List<TopicViewModel>();
            try
            {
                string html = Http.HttpHelper.DownloadHtml(pageUrl);
                if (html == null)
                {
                    throw new Exception($"页面{pageUrl}为空");
                }

                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(html);
                {
                    // //*[@id="ajaxtable"]/tbody[2]/tr[13]
                    string trPath = "//*[@id='ajaxtable']/tbody/tr";
                    var trArray = document.DocumentNode.SelectNodes(trPath);
                    if (trArray != null)
                    {
                        //bool startLoad = true;
                        //Parallel.ForEach(trArray, trNode => { 
                        foreach (var trNode in trArray)
                        {
                            string trHtml = trNode.OuterHtml;
                            HtmlDocument trHtmlDoc = new HtmlDocument();
                            trHtmlDoc.LoadHtml(trHtml);
                            {

                                //    //*[@id="ajaxtable"]/tbody[2]/tr[13]/td[2]/h3/a
                                string trUrlPath = "//*[@class='tr3 t_one tac']/td[2]/h3/a";
                                var imageNode = trHtmlDoc.DocumentNode.SelectSingleNode(trUrlPath);
                                if (imageNode != null)
                                {
                                    string titleUrl = null;
                                    string title = null;
                                    if (imageNode.Attributes["href"] != null)
                                    {
                                        titleUrl = imageNode.Attributes["href"].Value;
                                    }
                                    var font = imageNode.ChildNodes.FirstOrDefault();
                                    if (font.InnerText != null)
                                    {
                                        title = font.InnerText;
                                    }
                                    TopicViewModel topic = new TopicViewModel();
                                    topic.TopicName = title;
                                    topic.TopicUrl = titleUrl;
                                    topics.Add(topic);
                                }
                            }
                        }
                        // });
                    }
                    else
                    {
                        throw new Exception("没有找到节点");
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
            return topics;

        }
        public List<ImageViewModel> CrawlTopic(string topicUrl,out string topicName)
        {
            List<ImageViewModel> images = new List<ImageViewModel>();
            try
            {
                string html = HttpHelper.DownloadHtml(topicUrl);
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(html);
                var head= document.DocumentNode.SelectSingleNode("//*/head/title");
                topicName = head?.InnerText.Split('|')[0];
                //  //*[@id="main"]/div[3]/table/tbody/tr[1]/th[2]/table/tbody/tr/td
                //  //*[@id="main"]/div[3]/table/tbody/tr[1]/th[2]/table/tbody/tr/td/div[4]/p
                //string inputPath = "//*[@id='main']/div[3]/table/tr[1]/th[2]/table/tr/td/div[4]";
                string inputPath = "//*[@class='tpc_content do_not_catch']";
                var trArray = document.DocumentNode.SelectSingleNode(inputPath);
                if (trArray != null)
                {
                    string[] xPaths = new string[] { "//*/input", "//*/img", "/input" ,"/img"};
                    HtmlDocument docTrArray = new HtmlDocument();
                    docTrArray.LoadHtml(trArray.InnerHtml);
                    foreach (var xpath in xPaths)
                    {
                        var child = docTrArray.DocumentNode.SelectNodes(xpath);
                        images.AddRange(ImageLoad(child));
                    }
                }
                else
                {
                    string inputPath2 = "//*[@id='main']/div[3]/table/tr[1]/th[2]/table/tr/td/div[4]/input";
                    var trArray2 = document.DocumentNode.SelectNodes(inputPath2);
                    images.AddRange(ImageLoad(trArray2));
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{topicUrl}异常：" + ex.Message);
                throw;
            }
            return images;
        }

        private List<ImageViewModel> ImageLoad(HtmlNodeCollection nodes)
        {
            if (nodes != null)
            {
                List<ImageViewModel> images = new List<ImageViewModel>();
                foreach (var input in nodes)
                {
                    string imgurl = null;
                    if (input.Attributes["src"] != null)
                    {
                        imgurl = input.Attributes["src"].Value;
                    }
                    else if (input.Attributes["data-src"] != null)
                    {
                        imgurl = input.Attributes["data-src"].Value;
                    }
                    else if (input.Attributes["data-link"] != null)
                    {
                        imgurl = input.Attributes["data-link"].Value;
                    }
                    if (imgurl != null)
                    {
                        images.Add(new ImageViewModel()
                        {
                            ImageUrl = imgurl
                        });
                    }

                }
                return images;
            }
            else
            {
                return new List<ImageViewModel>();
                //throw new Exception("没有找到节点！");
            }
        }
    }
}
