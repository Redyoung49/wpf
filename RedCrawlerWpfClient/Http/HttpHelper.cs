using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace RedCrawlerWpfClient.Http
{
    public class HttpHelper
    {
        public static ILogger Logger => DefaultLogger.Instance;
        /// <summary>
        /// 下载html
        /// http://tool.sufeinet.com/HttpHelper.aspx
        /// HttpWebRequest功能比较丰富，WebClient使用比较简单
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string DownloadHtml(string url)
        {
            string html = string.Empty;
            try
            {
                //logger.Info($"准备下载{url}");
                //HttpClient

                ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;//模拟请求


                request.Timeout = 10 * 1000;//设置30s的超时
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";//pc浏览器
                                                                                                                                                    //request.UserAgent = "Ruanmou Crawler";
                                                                                                                                                    //request.UserAgent = "Mozilla / 5.0(iPhone; CPU iPhone OS 7_1_2 like Mac OS X) App leWebKit/ 537.51.2(KHTML, like Gecko) Version / 7.0 Mobile / 11D257 Safari / 9537.53";//移动端浏览器

                // request.ContentType = "text/html";// "text/html;charset=gbk";// 
                //request.Host = "";

                request.Headers.Add("Cookie", @"__cfduid=d4c0391648e529610623bd5943f3704111531269067; __guid=130464477.3597193739047036400.1531269068634.9253; UM_distinctid=16486bfd30e724-0af819c5d02c1a-5d4e211f-1fa400-16486bfd30ffe7; monitor_count=30; CNZZDATA950900=cnzz_eid%3D997946987-1531267108-https%253A%252F%252Fcl.uozvy.com%252F%26ntime%3D1531326508");

                // request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                // request.Headers.Add("Accept-Encoding", "gzip, deflate, sdch, br");
                //request.Headers.Add("Referer", "http://list.yhd.com/c0-0/b/a-s1-v0-p1-price-d0-f0-m1-rt0-pid-mid0-kiphone/");
                request.Method = "GET";
                //Encoding enc = Encoding.GetEncoding("GB2312"); // 如果是乱码就改成 utf-8 / GB2312

                //int sort = 2;//人数
                //string dataString = string.Format("k={0}&n=24&st={1}&iso=0&src=1&v=4093&p={2}&isRecommend=false&city_id=0&from=1&ldw=1361580739", keyword, sort, 1);
                //Encoding encoding = Encoding.UTF8;//根据网站的编码自定义  
                //byte[] postData = encoding.GetBytes(dataString);
                //request.ContentLength = postData.Length;
                //Stream requestStream = request.GetRequestStream();
                //requestStream.Write(postData, 0, postData.Length);

                Encoding enc = Encoding.GetEncoding("GBK");//.GetEncoding("GB2312");
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)//发起请求
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Logger.Warning(string.Format("抓取{0}地址返回失败,response.StatusCode为{1}", url, response.StatusCode));
                    }
                    else
                    {
                        try
                        {
                            StreamReader sr = new StreamReader(response.GetResponseStream(), enc);
                            html = sr.ReadToEnd();//读取数据
                            sr.Close();
                        }
                        catch (Exception ex)
                        {
                            Logger.Warning(string.Format($"DownloadHtml抓取{url}失败") + ex.Message);
                            html = null;
                        }
                    }
                }
            }
            catch (System.Net.WebException ex)
            {
                if (ex.Message.Equals("远程服务器返回错误: (306)。"))
                {
                    Logger.Warning("远程服务器返回错误: (306)。" + ex.StackTrace);
                    html = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Warning(string.Format("DownloadHtml抓取{0}出现异常", url) + ex.Message);
                html = null;
            }
            return html;
        }
        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
    }
}
