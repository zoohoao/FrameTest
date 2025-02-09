using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace AvalonDockTest
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            LoadLayout();
            var anchorable = dockManager.Layout.Descendents().OfType<LayoutAnchorable>()
                                .FirstOrDefault(a => a.ContentId == "Chanel1");

            if (anchorable != null)
            {
                anchorable.ToolTip = "Chanel1 Scan Image";
            }
        }

        public void SaveLayout()
        {
            var serializer = new XmlLayoutSerializer(dockManager);
            using (var stream = new StreamWriter(string.Format(@".\AvalonDock_{0}.config", "dock")))
                serializer.Serialize(stream);
        }

        public void LoadLayout()
        {
            var currentContentsList = dockManager.Layout.Descendents().OfType<LayoutContent>().Where(c => c.ContentId != null).ToArray();

            var serializer = new XmlLayoutSerializer(dockManager);
            using (var stream = new StreamReader(string.Format(@"C:\Users\Administrator\Desktop\SonicProject\FrameTest\AvalonDockTest\bin\Debug\AvalonDock_{0}.config", "dock")))
                serializer.Deserialize(stream);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveLayout();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            LoadLayout();
        }

        private void ButtonLoadImage_Click(object sender, RoutedEventArgs e)
        {
            image1.LoadFile(@"C:\Users\Administrator\Pictures\openCV/2.png");
        }

        private void GCClear(object sender, RoutedEventArgs e)
        {
            image1.ClearImage();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
}