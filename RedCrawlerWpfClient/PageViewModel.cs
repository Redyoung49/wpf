using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using RedCrawlerWpfClient.RedCrawler;

namespace RedCrawlerWpfClient
{
    public class PageViewModel : NotifyObject
    {
        RedCrawler.CrawlerHelper crawler;
        public PageViewModel()
        {
            Topics = new ObservableCollection<TopicViewModel>();
            crawler = new RedCrawler.CrawlerHelper();
            //CrawlTopics();
            
        }
        public string HostUrl { get; set; }
        private string _urlTemp;
        public string HostUrlTemp { get => _urlTemp; set {
                _urlTemp = value;
                OnPropertyChanged();
            } }
        public string DownloadUrl { get; set; }
        public int Fid { get; set; } = 16;
        /// <summary>
        /// 页面链接
        /// </summary>
        public string PageUrl { get {
                return $"thread0806.php?fid={Fid}&search=&page={PageNumber}";
            } }
        public string DownloadDirectory { get; set; } = "PhotosDownload";
        public void SetPageNumber(int pageNum)
        {
            PageNumber = pageNum;
        }
        private int _pageNumber=1;
        public int PageNumber { get=> _pageNumber; set {
                if (value>0)
                {
                    if (!string.IsNullOrEmpty( HostUrl))
                    {
                        _pageNumber = value;
                        CrawlTopics();
                        OnPropertyChanged();
                    }
                    
                }
            } }
        private int? _pageNumberTemp = 1;
        public int? PageNumberTemp
        {
            get => _pageNumberTemp; set
            {
                if (value > 0)
                {
                    _pageNumberTemp = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isCrawl = false;

        public bool IsCrawl
        {
            get { return _isCrawl; }
            set { _isCrawl = value;OnPropertyChanged(); }
        }
        private void CrossThreadInvoke(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }
        public async void CrawlTopics()
        {
            if (string.IsNullOrEmpty( HostUrl))
            {
                return;
            }
            if (!IsCrawl)
            {
                IsCrawl = true;
                var topics = await Task.Run(() => {
                    try
                    {
                        Uri uri = new Uri(HostUrl);
                        return crawler.CrawlPage(new Uri(uri, PageUrl).ToString());
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message + ex.StackTrace);
                        return new List<TopicViewModel>();
                    }
                });
                Topics.Clear();
                CrossThreadInvoke(() => {
                    foreach (var topic in topics)
                    {
                        topic.Page = this;
                        Topics.Add(topic);
                    }
                });
                IsCrawl = false;
                Logger.Info($"{PageUrl}加载完成！");
            }
            
        }
        
        public void JumpToPage(int pageNum)
        {
            if (pageNum>0)
            {
                PageNumber = pageNum;
            }
        }
        public ILogger Logger => DefaultLogger.Instance;
        /// <summary>
        /// 主题表
        /// </summary>
        public ObservableCollection<TopicViewModel> Topics { get; set; }
        /// <summary>
        /// 访问
        /// </summary>
        public RelayWithoutParamCommand SendCommand
        {
            get
            {
                return new RelayWithoutParamCommand(() => {
                    if (string.IsNullOrEmpty( HostUrlTemp))
                    {
                        Logger.Warning("地址不能为空！");
                        return;
                    }
                    var hosttemp = HostUrlTemp.ToLower();
                    if (!hosttemp.StartsWith("http://")&& !hosttemp.StartsWith("https://"))
                    {
                        hosttemp = "http://" + hosttemp;
                    }
                    HostUrl = hosttemp;
                    JumpToPage(1);
                });
            }
        }
        /// <summary>
        /// 下一页
        /// </summary>
        public RelayWithoutParamCommand NextPage { get{
                return new RelayWithoutParamCommand(()=> {
                    PageNumber++;
                });
            } }
        /// <summary>
        /// 上一页
        /// </summary>
        public RelayWithoutParamCommand PrePage
        {
            get
            {
                return new RelayWithoutParamCommand(() => {
                    PageNumber--;
                });
            }
        }
        /// <summary>
        /// 跳转
        /// </summary>
        public RelayWithoutParamCommand JumpTo
        {
            get
            {
                return new RelayWithoutParamCommand(() => {
                    if (PageNumberTemp.HasValue)
                    {
                        if (PageNumber != PageNumberTemp.Value)
                        {
                            JumpToPage(PageNumberTemp.Value);
                        }
                    }
                });
            }
        }
        /// <summary>
        /// 打开当前目录
        /// </summary>
        public RelayWithoutParamCommand OpenFloadCommand
        {
            get
            {
                return new RelayWithoutParamCommand(() => {
                    if (DownloadDirectory.Equals("PhotosDownload"))
                    {
                        System.Diagnostics.Process.Start("explorer.exe", AppDomain.CurrentDomain.BaseDirectory);
                        return;
                    }
                    System.Diagnostics.Process.Start("explorer.exe", DownloadDirectory);

                });
            }
        }
        /// <summary>
        /// 选择文件目录
        /// </summary>
        public RelayWithoutParamCommand OpenSaveFloder
        {
            get
            {
                return new RelayWithoutParamCommand(() => {
                    System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
                    folderBrowserDialog.Description = "选择保存图片的文件夹";
                    folderBrowserDialog.ShowNewFolderButton = true;
                    if (DownloadDirectory.Equals("PhotosDownload"))
                    {
                        folderBrowserDialog.SelectedPath =  AppDomain.CurrentDomain.BaseDirectory;
                    }
                    else
                    {
                        folderBrowserDialog.SelectedPath = DownloadDirectory;
                    }
                    
                    if (folderBrowserDialog.ShowDialog()== System.Windows.Forms.DialogResult.OK)
                    {
                        DownloadDirectory = folderBrowserDialog.SelectedPath;
                        OnPropertyChanged(nameof(DownloadDirectory));
                    }
                });
            }
        }
        /// <summary>
        /// 全部下载
        /// </summary>
        public RelayWithoutParamCommand DowmloadAllCommand
        {
            get
            {
                return new RelayWithoutParamCommand(() => {
                    List<Task> tasks = new List<Task>();
                    foreach (var topic in Topics)
                    {
                        Logger.Info($"{topic.TopicName}开始下载...");
                        tasks.Add( topic.CrawlImagesAsync(System.IO.Path.Combine(this.HostUrl, topic.TopicUrl)));
                    }
                    Task.WhenAll(tasks.ToArray()).ContinueWith(x=> {
                        Logger.Info($"{PageUrl}全部下载完成！");
                    });
                });
            }
        }
        /// <summary>
        /// 全部取消
        /// </summary>
        public RelayWithoutParamCommand CancelAllCommand
        {
            get
            {
                return new RelayWithoutParamCommand(() => {
                    foreach (var topic in Topics)
                    {
                        topic.CrawlCancel();
                    }
                    Logger.Info($"全部下载已取消，等待自动结束...");
                });
            }
        }
        /// <summary>
        /// 刷新
        /// </summary>
        public RelayWithoutParamCommand RefreshCommand
        {
            get
            {
                return new RelayWithoutParamCommand(() => {
                    CrawlTopics();
                });
            }
        }
        /// <summary>
        /// 页面分类访问
        /// </summary>
        public RelayCommand PageClassCommand { get => new RelayCommand(param=> {
            try
            {
                int fid = Convert.ToInt32(param);
                if (Fid==fid)
                {
                    return;
                }
                Fid = fid;
                CrawlTopics();
            }
            catch (Exception ex)
            {
                Logger.Warning(ex.Message);
                Fid = 16;
            }

        }); }
        /// <summary>
        /// 下载指定链接
        /// </summary>
        public RelayWithoutParamCommand DownloadUrlCommand => new RelayWithoutParamCommand(async ()=> {
            var url = DownloadUrl.Trim();
            if (!string.IsNullOrEmpty(url))
            {
                TopicViewModel topic = new TopicViewModel();
                topic.PropertyChanged += (x, e) => {
                    if (e.PropertyName.Equals(nameof(topic.TopicName)))
                    {
                        Logger.Info($"地址{url}的主题名称为：{topic.TopicName}");
                    }
                };
                topic.Page = this;
                await topic.CrawlImagesAsync(url);
                Logger.Info($"{topic.TopicName}：下载完成！");
            }
        });
        public CrawlerHelper Crawler { get => crawler;}
    }
}
