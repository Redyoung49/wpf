using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace RedCrawler.Test
{
    public class DownloadImageTest
    {
        public ILogger Logger => DefaultLogger.Instance;
        public void DownloadFile(string url,string directory)
        {
            string filename = Path.GetFileName(url);
            var path = Path.Combine(directory, filename);
            if (!File.Exists(path))
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    Logger.Debug($" 当前目录{directory}\t正在下载：{url}");
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                    req.Method = "GET";

                    var response = req.GetResponse().GetResponseStream();


                    using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        byte[] buffer = new byte[512];
                        int result = 0;
                        while ((result = response.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fs.Write(buffer, 0, result);
                        }
                        fs.Flush();
                        fs.Close();
                    }
                    response.Close();
                }
            }
            
        }
    }
}
