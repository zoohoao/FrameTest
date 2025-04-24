using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace ImageView.Views
{
    /// <summary>
    /// ImageView.xaml 的交互逻辑
    /// </summary>
    public partial class ImageView : UserControl
    {
        public ImageView()
        {
            InitializeComponent();
            img1.Source = new BitmapImage(new Uri("C:\\Users\\Administrator\\Source\\Repos\\FrameTest\\ImageView\\Views\\1.png", UriKind.Absolute));
        }

        private bool isMouseLeftButtonDown = false;

        private Point previousMousePoint;
        private Point position;

        private void Img_MouseDown1(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                return;
            }
            isMouseLeftButtonDown = true;
            previousMousePoint = e.GetPosition(mainBox1);
        }

        private void Img_MouseLeave1(object sender, MouseEventArgs e)
        {
            isMouseLeftButtonDown = false;
        }

        private void Img_MouseMove1(object sender, MouseEventArgs e)
        {
            position = e.GetPosition(mainBox1);

            if (isMouseLeftButtonDown == true)
            {
                double offX = position.X - previousMousePoint.X;
                double offY = position.Y - previousMousePoint.Y;
                var scal = view.ActualHeight / img1.Source.Width;
                var offX1 = (ActualWidth - view.ActualHeight) / 2 / scal;
                //变换矩阵
                var newMatrix = new Matrix(1, 0, 0, 1, offX, offY);

                var tempMatrix = newMatrix * matrix.Matrix;

                matrix.Matrix = tempMatrix;
            }
        }

        private void Img_MouseUp1(object sender, MouseButtonEventArgs e)
        {
            isMouseLeftButtonDown = false;
        }

        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //鼠标位置
            Point p = e.GetPosition(mainBox1);

            //bs 缩放系数 e.Delta 上滚120 & 下滚-120
            double bs = 1 + e.Delta * 0.001;

            //相对鼠标的移动量
            double offX = p.X - p.X * bs;
            double offY = p.Y - p.Y * bs;

            //变换矩阵
            var newMatrix = new Matrix(bs, 0, 0, bs, offX, offY);

            //
            if ((newMatrix * matrix.Matrix).M11 <= 1 || (newMatrix * matrix.Matrix).M11 > 64)
            {
                return;
            }
            matrix.Matrix = newMatrix * matrix.Matrix;
        }

        private void view_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(view);
        }
    }
}