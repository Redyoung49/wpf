using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RedCrawlerWpfClient
{
    /// <summary>
    /// PageListView.xaml 的交互逻辑
    /// </summary>
    public partial class PageListView : UserControl
    {
        public PageListView()
        {
            InitializeComponent();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer viewer)
            {
                double num = Math.Abs((int)(e.Delta / 2));
                double offset = 0.0;
                if (e.Delta > 0)
                {
                    offset = Math.Max((double)0.0, (double)(viewer.VerticalOffset - num));
                }
                else
                {
                    offset = Math.Min(viewer.ScrollableHeight, viewer.VerticalOffset + num);
                }
                if (offset != viewer.VerticalOffset)
                {
                    viewer.ScrollToVerticalOffset(offset);
                    e.Handled = true;
                }
            }
            
        }

        private void ListBox_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            if (sender is ListBox listBox)
            {
                Decorator decorator = (Decorator)VisualTreeHelper.GetChild(listBox, 0);
                ScrollViewer scrollViewer = (ScrollViewer)decorator.Child;
                scrollViewer.ScrollToEnd();
            }
            

        }
    }
}
