using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedCrawler.Test;
using RedCrawler.Crawler;
using System.Threading;

namespace RedCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ClCrawer crawler = new ClCrawer(@"https://cl.uozvy.com/");
                ThreadPool.SetMinThreads(20, 20);
                for (int i = 41; i < 70; i++)
                {
                    DefaultLogger.Instance.Info($"》》》》》》》》》《《《第{i}页图片抓取开始！》》》》》》》》》》》》");
                    crawler.Crawl(@"thread0806.php?fid=16&search=&page="+i);
                    DefaultLogger.Instance.Info($"》》》》》》》》》》》》第{i}页图片抓取完成！《《《《《《《《《《《《");
                }
                DefaultLogger.Instance.Info("爬虫抓取完成！");
            }
            catch (Exception e)
            {
                Console.WriteLine("异常了 完了！"+e.Message+e.StackTrace);
                
            }
            DefaultLogger.Instance.Info("程序完成，回车完成退出！");
            Console.ReadLine();
        }
    }
}
