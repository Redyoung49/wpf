using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;

namespace RedCrawlerWpfClient
{
    public class TopicViewModel : NotifyObject
    {
        public ILogger Logger => DefaultLogger.Instance;
        public TopicViewModel()
        {
            Images = new ObservableCollection<ImageViewModel>();
        }
        private System.Threading.CancellationTokenSource tokenSource;
        public async Task CrawlImagesAsync(string topicUrl)
        {
            if (IsCrawl)
            {
                return;
            }
            if (CurrentDown>0&&CurrentDown==Images.Count)
            {
                return;
            }
            if (Page!=null)
            {
                tokenSource = new System.Threading.CancellationTokenSource();
                try
                {
                    IsCrawl = true;
                    var images = await Task.Run(() =>
                    {
                        //获取所有图片
                        var imgs = Page.Crawler.CrawlTopic(topicUrl, out string name);
                        if (string.IsNullOrEmpty(TopicName))
                        {
                            TopicName = $"{name}";
                        }
                        return imgs;
                    });
                    foreach (var image in images)
                    {
                        image.Topic = this;
                        Images.Add(image);
                    }
                    if (!System.IO.Directory.Exists(TopicPath))
                    {
                        System.IO.Directory.CreateDirectory(TopicPath);
                    }
                    await Task.Run(() => {
                        foreach (var image in Images)
                        {
                            try
                            {
                                if (tokenSource.IsCancellationRequested)
                                {
                                    return;
                                }
                                image.CrawlImage();
                                OnPropertyChanged(nameof(CurrentDown));
                            }
                            catch (Exception ex)
                            {
                                Logger.Error("图片：" +image.ImageUrl + "路径：" + image.ImagePath + " " + ex.Message);
                            }
                            
                        }
                    }).ContinueWith(t => {
                        IsCrawl = false;
                        if (!tokenSource.IsCancellationRequested)
                        {
                            Logger.Info($"主题{TopicName}已下载完成。");
                            IsDownloadOk = true;
                        }
                        else
                        {
                            Logger.Info($"主题{TopicName}已取消下载。");
                        }
                        tokenSource = null;
                    });
                   
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }

                
            }
        }
        public void CrawlCancel()
        {
            if (IsCrawl)
            {
                if (tokenSource != null)
                {
                    tokenSource.Cancel();
                }
            }
        }
        private string _topicpath;
        public string TopicPath { get {
                if (string.IsNullOrEmpty(_topicpath))
                {
                    _topicpath = System.IO.Path.Combine(Page.DownloadDirectory,TopicName);
                }
                return _topicpath;
            } }
        public PageViewModel Page { get; set; }
        private bool _isCrawl;

        public bool IsCrawl
        {
            get { return _isCrawl; }
            set { _isCrawl = value; OnPropertyChanged();
            }
        }
        public int CurrentDown
        {
            get => Images.Count(i => i.IsExist);
        }
        private bool _isDownloadOk;

        public bool IsDownloadOk
        {
            get { return _isDownloadOk; }
            set { _isDownloadOk = value;
                OnPropertyChanged();
            }
        }

        private string _topicName;

        public string TopicName
        {
            get { return _topicName; }
            set { _topicName = value;OnPropertyChanged(); }
        }
        public string TopicUrl { get; set; }

        public ObservableCollection<ImageViewModel> Images { get; }
        public RelayWithoutParamCommand DownloadCommand
        {
            get
            {
                return new RelayWithoutParamCommand(async () => {
                    Logger.Info($"{TopicName}开始下载...");
                    
                    await CrawlImagesAsync(System.IO.Path.Combine(Page.HostUrl, TopicUrl));
                });
            }
        }
        public RelayWithoutParamCommand CancelCommand
        {
            get
            {
                return new RelayWithoutParamCommand(() => {
                    if (IsCrawl&&tokenSource!=null)
                    {
                        Logger.Info($"{TopicName}取消下载...");
                        tokenSource.Cancel();
                    }
                    
                });
            }
        }
        public RelayWithoutParamCommand OpenFloadCommand
        {
            get
            {
                return new RelayWithoutParamCommand(() => {
                    System.Diagnostics.Process.Start("explorer.exe", TopicPath);

                });
            }
        }
        public RelayWithoutParamCommand CopyUrlCommand
        {
            get => new RelayWithoutParamCommand(() => {
                try
                {
                    string url = System.IO.Path.Combine(Page.HostUrl, TopicUrl);
                    Clipboard.SetDataObject(url);
                    Logger.Info(url+"已复制到剪切板。");
                }
                catch (Exception ex)
                {
                    Logger.Warning(ex.Message);
                }
                
            });
        }
    }
}
