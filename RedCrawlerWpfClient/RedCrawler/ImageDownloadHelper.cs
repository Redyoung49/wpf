using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RedCrawlerWpfClient.RedCrawler
{
    public class ImageDownloadHelper
    {
        public ILogger Logger => DefaultLogger.Instance;
        public void DownloadFile(string imageUrl, string imageSavePath)
        {
            //if (!File.Exists(imageSavePath))
            {
                //if (!Directory.Exists(directory))
                {
                    //Directory.CreateDirectory(directory);
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(imageUrl);
                    req.ReadWriteTimeout = 10 * 1000;
                    req.Method = "GET";
                    var response = req.GetResponse().GetResponseStream();
                    using (FileStream fs = new FileStream(imageSavePath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        byte[] buffer = new byte[1024];
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
