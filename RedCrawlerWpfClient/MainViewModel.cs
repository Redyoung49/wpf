using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RedCrawlerWpfClient
{
    public class MainViewModel :NotifyObject
    {
        public MainViewModel()
        {
            Messages = new System.Collections.ObjectModel.ObservableCollection<LogItem>();
            DefaultLogger.Instance.LoggerEvent += Instance_LoggerEvent;
        }
        private void CrossThreadInvoke(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }
        private void Instance_LoggerEvent(object arg1, LogLevel arg2, string arg3)
        {
            if (Messages.Count>500)
            {
                Messages.RemoveAt(0);
            }
            CrossThreadInvoke(()=> {
                Messages.Add(new LogItem() { Message = arg3 });
            });
            
        }
        public System.Collections.ObjectModel.ObservableCollection<LogItem> Messages { get;  }
    }
    public class LogItem
    {
        public LogLevel Level { get; set; }
        public string Message { get; set; }
    }
}
