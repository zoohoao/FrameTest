using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ImageDoing
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        // 是否处于拖拽状态
        private bool _isPanning = false;

        // 记录上一次鼠标位置(在 Canvas 的坐标系里)
        private Point _previousMousePos;

        public MainWindow()
        {
            InitializeComponent();
        }

        // 当前的缩放系数
        private double zoomFactor = 1;

        internal double ZoomFactor
        {
            get
            {
                return zoomFactor;
            }
            set
            {
                value = value > 64 ? 64 : value;
                value = (float)(value < 1 ? 1 : value);
                zoomFactor = value;
            }
        }

        private void MyCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // 1) 取得鼠标在 Canvas 坐标下的位置
            Point mousePos = e.GetPosition(MyCanvas);

            // 2) 计算缩放前，鼠标对应“图像坐标”的位置 (scaledCenterBefore)
            //    先把平移去掉，再除以当前缩放。
            //    这样可以得到：在旧 zoomFactor 下，鼠标对应图像的 "局部坐标"
            double beforeX = (mousePos.X - translateTransform.X) / scaleTransform.ScaleX;
            double beforeY = (mousePos.Y - translateTransform.Y) / scaleTransform.ScaleY;

            // 3) 更新 zoomFactor (每次滚轮 20%)
            if (e.Delta > 0) // 滚轮向上，放大
            {
                ZoomFactor *= 1.2;
            }
            else // 滚轮向下，缩小
            {
                ZoomFactor *= 0.8;
            }

            // 4) 设置新的缩放
            scaleTransform.ScaleX = ZoomFactor;
            scaleTransform.ScaleY = ZoomFactor;

            // 5) 计算缩放后，鼠标对应的图像坐标 (scaledCenterAfter)
            double afterX = (mousePos.X - translateTransform.X) / scaleTransform.ScaleX;
            double afterY = (mousePos.Y - translateTransform.Y) / scaleTransform.ScaleY;

            // 6) 计算差值 (Delta)
            //    如果我们什么都不做，那么当缩放改变后 (before -> after)，
            //    鼠标指向的点在屏幕上会发生偏移。我们要对平移做补偿，
            //    保持鼠标所在的图像像素点 "依旧位于鼠标位置"。
            double diffX = afterX - beforeX;
            double diffY = afterY - beforeY;

            // 7) 平移补偿。要让图像 "反向移动" 这个差值在屏幕上的量。
            //    因为 scaleTransform 已经改变，这里的 diffX/ diffY 是图像坐标的差，
            //    转化到屏幕像素上，也可以直接加到 translateTransform.X/Y。
            translateTransform.X += diffX * scaleTransform.ScaleX;
            translateTransform.Y += diffY * scaleTransform.ScaleY;
            ClampTranslate();
            // 实时更新文本
            Point mousePos1 = e.GetPosition(MyCanvas);
            // InfoTextBlock.RenderTransform = new ScaleTransform(scaleTransform.ScaleX, scaleTransform.ScaleY);
            UpdateInfoText(mousePos1);
        }

        // 按下左键，开启拖拽
        private void MyCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isPanning = true;
            // 记录此刻鼠标位置
            _previousMousePos = e.GetPosition(MyCanvas);
            // 捕获鼠标，让后续 MouseMove 能继续触发
            MyCanvas.CaptureMouse();
        }

        // 松开左键，停止拖拽
        private void MyCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isPanning = false;
            MyCanvas.ReleaseMouseCapture();
        }

        // 拖拽进行中，更新平移
        private void MyCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            // 实时更新文本
            Point currentPos = e.GetPosition(MyCanvas);
            if (_isPanning)
            {
                // 计算本次移动差值
                double deltaX = currentPos.X - _previousMousePos.X;
                double deltaY = currentPos.Y - _previousMousePos.Y;

                // 直接加到 TranslateTransform 上
                translateTransform.X += deltaX;
                translateTransform.Y += deltaY;
                // 更新 _previousMousePos
                _previousMousePos = currentPos;
                // 再Clamp一下
                ClampTranslate();
            }
            // 鼠标停留时，无论是否拖拽，都可显示坐标
            UpdateInfoText(currentPos);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CenterImage();
        }

        /// <summary>
        /// 根据当前的 Canvas 大小和 Image 的大小，自动让图像居中
        /// </summary>
        private void CenterImage()
        {
            translateTransform.X = 0;
            translateTransform.Y = 0;
            ZoomFactor = 1;
            scaleTransform.ScaleX = 1;
            scaleTransform.ScaleY = 1;
            MyCanvas.Width = MyImage.Width;
            MyCanvas.Height = MyImage.Height;
            MyImage.Stretch = Stretch.Uniform;
            // 强制刷新布局
            MyImage.UpdateLayout();
            // 1) Canvas 的实际大小
            double canvasWidth = MyCanvas.ActualWidth;
            double canvasHeight = MyCanvas.ActualHeight;

            if (canvasWidth <= 0 || canvasHeight <= 0)
                return; // 避免初始还没布局完出现负值

            // 2) 图片的原始大小 (这里可以用 MyImage.Source.Width/Height,
            //    也可以用 MyImage.ActualWidth/ActualHeight；如果 Image 没固定宽高，则需小心)
            //    假如 MyImage 并没有被固定 Width/Height，则 MyImage.ActualWidth/ActualHeight
            //    会是加载后的实际大小。
            double imageOriginalWidth = MyImage.ActualWidth;
            double imageOriginalHeight = MyImage.ActualHeight;

            // 如果实际还没加载完，就先返回
            if (imageOriginalWidth <= 0 || imageOriginalHeight <= 0)
                return;

            // 3) 计算在当前缩放下，图片的显示宽高
            //    scaleTransform.ScaleX / ScaleY 就是当前缩放倍数 _zoomFactor
            double displayedWidth = imageOriginalWidth * scaleTransform.ScaleX;
            double displayedHeight = imageOriginalHeight * scaleTransform.ScaleY;

            // 4) 计算居中偏移
            double offsetX = (canvasWidth - displayedWidth) / 2;
            double offsetY = (canvasHeight - displayedHeight) / 2;

            // 5) 设置到 TranslateTransform
            translateTransform.X = offsetX;
            translateTransform.Y = offsetY;
        }

        // ========== 核心: 限制图像不出窗口（Canvas） ==========
        private void ClampTranslate()
        {
            //  Canvas 尺寸
            double canvasW = MyCanvas.ActualWidth;
            double canvasH = MyCanvas.ActualHeight;
            if (canvasW <= 0 || canvasH <= 0) return;

            //  图片在当前缩放下的宽高
            //    如果 MyImage 没固定 width/height，就用 MyImage.ActualWidth/Height 获取真实大小
            double imgW = MyImage.ActualWidth;
            double imgH = MyImage.ActualHeight;

            if (imgW <= 0 || imgH <= 0) return;

            double scaledW = imgW * scaleTransform.ScaleX;
            double scaledH = imgH * scaleTransform.ScaleY;

            // 计算 X/Y 的最小值与最大值
            // 当 scaledW < canvasW 时，maxX= (canvasW - scaledW) 将是正值 => 允许向右移动
            // 当 scaledW > canvasW 时，(canvasW - scaledW) 为负 => 允许向左移动一部分
            double minX = Math.Min(0, canvasW - scaledW);
            double maxX = Math.Max(0, canvasW - scaledW);
            double minY = Math.Min(0, canvasH - scaledH);
            double maxY = Math.Max(0, canvasH - scaledH);

            // 当前平移
            double x = translateTransform.X;
            double y = translateTransform.Y;

            // 夹取(Clamp)
            if (x > maxX) x = maxX;

            if (x < minX) x = minX;
            if (y > maxY) y = maxY;
            if (y < minY) y = minY;

            translateTransform.X = x;
            translateTransform.Y = y;
        } // =========== 4. 在文本框显示缩放倍数 & 鼠标坐标 ===========

        private void UpdateInfoText(Point mousePosCanvas)
        {
            // 缩放倍数(如 "125%")
            string zoomText = $"{(ZoomFactor * 100.0):F0}%";

            // 计算鼠标在 "图片坐标" 上的像素坐标
            //  1) 去掉 translateTransform
            //  2) 再除以 scaleTransform
            double imgPosX = (mousePosCanvas.X - translateTransform.X) / scaleTransform.ScaleX;
            double imgPosY = (mousePosCanvas.Y - translateTransform.Y) / scaleTransform.ScaleY;

            // 这里再判断一下，如果图像还没加载，会出现无效值
            double imgW = MyImage.ActualWidth;
            double imgH = MyImage.ActualHeight;

            // 如果鼠标在图像可见范围内，才显示坐标
            // 你可以根据实际需求，看是否要限制到 0 ~ imgW / imgH 之内
            //if (imgPosX >= 0 && imgPosX < imgW && imgPosY >= 0 && imgPosY < imgH)
            {
                // 像素坐标通常保留整数，如果需要小数可自己改
                int pixelX = (int)imgPosX;
                int pixelY = (int)imgPosY;
                InfoTextBlock.Text = $"倍数: {zoomText}  像素: ({pixelX}, {pixelY})  x偏移: {translateTransform.X} y偏移: {translateTransform.Y}";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // 设置本地图片路径
            string imagePath = @"C:\Users\Administrator\Pictures\openCV/2.png"; ;

            // 创建 BitmapImage 并加载图片
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
            bitmap.EndInit();
            LoadImageWithEffect(imagePath); ;
        }

        private WriteableBitmap writeableBitmap;
        private byte[] pixelData;
        private int imageWidth, imageHeight;
        private int currentRow = 0;
        private DispatcherTimer timer;

        private void LoadImageWithEffect(string filePath)
        {
            // 加载图片并确保像素数据加载完成
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(filePath, UriKind.Absolute);
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad; // 确保加载完成
            bitmapImage.EndInit();

            // 获取图片尺寸
            imageWidth = bitmapImage.PixelWidth;
            imageHeight = bitmapImage.PixelHeight;
            MyCanvas.Width = imageWidth;
            MyCanvas.Height = imageHeight;
            // 读取像素数据
            pixelData = new byte[imageWidth * imageHeight * 4]; // BGRA 格式
            bitmapImage.CopyPixels(pixelData, imageWidth * 4, 0);
            // 填充阿尔法通道
            for (int i = 0; i < pixelData.Length; i += 4)
            {
                pixelData[i + 3] = 255; // 将 Alpha 通道设置为不透明
            }
            // 创建 WriteableBitmap
            writeableBitmap = new WriteableBitmap(imageWidth, imageHeight, 96, 96, PixelFormats.Bgra32, null);
            MyImage.Stretch = Stretch.Uniform;
            MyImage.Source = writeableBitmap;
            // 强制刷新布局
            MyImage.UpdateLayout();
            CenterImage();
            // 初始化行计数器
            currentRow = 0;

            // 设置定时器逐行更新
            if (timer != null)
            {
                timer.Stop();
            }

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1) // 每 10ms 更新一行
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // 设置本地图片路径
            string imagePath = @"C:\Users\Administrator\Pictures\openCV/1.png"; ;

            // 创建 BitmapImage 并加载图片
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
            bitmap.EndInit();
            LoadImageWithEffect(imagePath); ;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (currentRow >= imageHeight)
            {
                timer.Stop(); // 图片加载完成，停止定时器
                return;
            }

            // 将当前行的像素数据写入 WriteableBitmap
            Int32Rect rect = new Int32Rect(0, 0, imageWidth, currentRow); // 定义一行的矩形区域
            int stride = imageWidth * 4; // 每行的字节数
            writeableBitmap.WritePixels(rect, pixelData, stride, 0);

            // 更新到下一行
            currentRow++;
        }
    }
}