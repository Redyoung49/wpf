using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RedCrawlerWpfClient
{
    public class ImageViewModel : NotifyObject
    {
        public ImageViewModel()
        {

        }
        public void CrawlImage()
        {
            RedCrawler.ImageDownloadHelper downloadHelper = new RedCrawler.ImageDownloadHelper();
            downloadHelper.DownloadFile(ImageUrl, ImagePath);
            OnPropertyChanged(nameof(IsExist));

        }
        public TopicViewModel Topic { get; set; }
        public string ImageUrl { get; set; }


        
        private string _imagepath;
        public string ImagePath { get{
                if (string.IsNullOrEmpty(_imagepath))
                {
                    _imagepath = System.IO.Path.Combine(Topic.Page.DownloadDirectory, Topic.TopicName, System.IO.Path.GetFileName(ImageUrl));
                }
                return _imagepath;
            } }
        private bool _isExist;
        public bool IsExist
        {
            get {
                if (_isExist)
                {
                    return true;
                }
                _isExist = System.IO.File.Exists(ImagePath);
                return _isExist;
            }
        }
        
    }
}
