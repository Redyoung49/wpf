using HtmlAgilityPack;
using RedCrawler.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedCrawler.Crawler
{
    public class ClCrawer
    {
        public ILogger Logger => DefaultLogger.Instance;
        private Test.DownloadImageTest downloadImage;
        private string _host;
        private string filepath = @"F:\Download";
        public ClCrawer(string host)
        {
            this._host = host;
            downloadImage = new Test.DownloadImageTest();
        }
        public void Crawl(string url)
        {
            try
            {
                string requestUrl = System.IO.Path.Combine(_host,url);
                Logger.Info($"请求页面：{requestUrl}");
                string html = HttpHelper.DownloadHtml(requestUrl);
                if (html==null)
                {
                    Logger.FatalError($"页面{requestUrl}为空");
                    return;
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
                            //if (!startLoad)
                            //{
                            //    string trInerPath = "//*[@class='tr2']";
                            //    var imageNode = trHtmlDoc.DocumentNode.SelectSingleNode(trInerPath);
                            //    if (imageNode != null)
                            //    {
                            //        startLoad = true;
                            //            return;
                            //    }
                            //}
                            //else
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
                                    CrawlTie(title, titleUrl);
                                }
                            }
                        }
                       // });
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message );
            }
           

        }
        public void CrawlTie(string title, string url)
        {
            Logger.Info($"进入主题：{title}\t地址：{url}");
            try
            {
                string requestUrl = System.IO.Path.Combine(_host, url);
                string html = HttpHelper.DownloadHtml(requestUrl);
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(html);

                //  //*[@id="main"]/div[3]/table/tbody/tr[1]/th[2]/table/tbody/tr/td/div[4]/p
                string inputPath = "//*[@id='main']/div[3]/table/tr[1]/th[2]/table/tr/td/div[4]/table/tr/td";
                var trArray = document.DocumentNode.SelectSingleNode(inputPath);
                if (trArray != null)
                {
                    var child = trArray.SelectNodes("//*/b/input");
                    ImageLoad(title, child);
                }
                else
                {
                    string inputPath2 = "//*[@id='main']/div[3]/table/tr[1]/th[2]/table/tr/td/div[4]/input";
                    var trArray2 = document.DocumentNode.SelectNodes(inputPath2);
                    ImageLoad(title, trArray2);

                }
                Logger.Info($"主题：{title}  下载完成。。。");
            }
            catch (Exception ex)
            {
                Logger.Error($"{System.IO.Path.Combine(_host, url)}"+ex.Message);
            }
        }
        private void ImageLoad(string title, HtmlNodeCollection nodes)
        {
            if (nodes!=null)
            {
                Parallel.ForEach(nodes, input => {
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
                        try
                        {
                            downloadImage.DownloadFile(imgurl, System.IO.Path.Combine(filepath, title));
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"{imgurl}{ex.Message}" );
                        }
                        
                    }
                });
            }
        }
    }
}
