using System.Drawing;
using System.Linq;
using System.Windows;

namespace ImageDoing
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonApplyRed_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ButtonToggleRect_Click(object sender, RoutedEventArgs e)
        {
            image1.TestAddRectangles();
        }

        private void ButtonShowHideRed_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            var rects = image1.canvasContainer.Children.OfType<System.Windows.Shapes.Rectangle>().ToList();
            foreach (var rect in rects)
            {
                image1.canvasContainer.Children.Remove(rect);
            }
        }

        private void ButtonLoadImage_Click(object sender, RoutedEventArgs e)
        {
            image1.LoadFile(@"C:\Users\Administrator\Pictures\openCV/2.png");
        }

        private void AddRectangle(object sender, RoutedEventArgs e)
        {
        }
    }
}