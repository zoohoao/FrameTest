using ImageDoing.Models;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using PointF = System.Drawing.PointF;

namespace ImageDoing
{
    /// <summary>
    /// ImageShowView.xaml 的交互逻辑
    /// </summary>
    public partial class ImageViewerView : UserControl
    {
        public ImageViewerView()
        {
            InitializeComponent();
        }

        // 是否处于拖拽状态
        private bool _isPanning = false;

        // 记录上一次鼠标位置(在 Canvas 的坐标系里)
        private Point _previousMousePos;

        /// <summary>
        /// 自适应后的图片缩放比例
        /// </summary>
        private double PicFactor { get; set; } = 1;

        #region 定义依赖属性

        //  ZoomFactor 缩放系数
        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor", typeof(double), typeof(ImageViewerView), new PropertyMetadata(1.0));

        // CLR 包装属性
        public double ZoomFactor
        {
            get => (double)GetValue(ZoomFactorProperty);
            set
            {
                value = value > 64 ? 64 : value;
                value = (double)(value < PicFactor ? PicFactor : value);
                SetValue(ZoomFactorProperty, value);
            }
        }

        /// <summary>
        ///   CurrentPoint,鼠标相对于图片所在的像素坐标
        /// </summary>
        public static readonly DependencyProperty CurrentPointProperty =
            DependencyProperty.Register("CurrentPoint", typeof(PointF), typeof(ImageViewerView), new PropertyMetadata(new PointF(0, 0)));

        // CLR 包装属性
        public PointF CurrentPoint
        {
            get => (PointF)GetValue(CurrentPointProperty);
            set => SetValue(CurrentPointProperty, value);
        }

        #endregion 定义依赖属性

        #region 图片缩放和平移

        private void MyCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (MyImage.Source == null)
            {
                return;
            }
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

            foreach (var child in canvasContainer.Children)
            {
                if (child is Shape shape)
                {
                    shape.StrokeThickness = 1 / ZoomFactor;
                    // 如果每个图形有不同的原始线宽，可以把它们存储在一个字典中再读取
                }
            }
            //限制图像不出窗口
            ClampTranslate();
            // 实时更新文本
            UpdateInfoText(mousePos);
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
            if (MyImage.Source == null)
            {
                return;
            }
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
            }// 右键画矩形
            if (_isRectSelecting)
            {
                if (!_selector.IsSelecting) return;
                var pt = e.GetPosition(canvasContainer);
                _selector.Update(pt, ZoomFactor);
            }
            // 鼠标停留时，无论是否拖拽，都可显示坐标
            UpdateInfoText(currentPos);
        }

        /// <summary>
        /// 加载图片居中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (MyImage.Source != null)
            {
                AdjustImageToFit(MyImage.Source.Width, MyImage.Source.Height);
            }
        }

        // ========== 核心: 限制图像不出窗口（Canvas） ==========
        private void ClampTranslate()
        {
            double canvasW = MyCanvas.ActualWidth;
            double canvasH = MyCanvas.ActualHeight;
            if (canvasW <= 0 || canvasH <= 0) return;

            double imgW = MyImage.ActualWidth;
            double imgH = MyImage.ActualHeight;
            if (imgW <= 0 || imgH <= 0) return;

            double scaledW = imgW * scaleTransform.ScaleX;
            double scaledH = imgH * scaleTransform.ScaleY;

            double minX, maxX, minY, maxY;

            if (scaledW <= canvasW)
            {
                minX = maxX = (canvasW - scaledW) / 2;
            }
            else
            {
                minX = canvasW - scaledW;
                maxX = 0;
            }

            if (scaledH <= canvasH)
            {
                minY = maxY = (canvasH - scaledH) / 2;
            }
            else
            {
                minY = canvasH - scaledH;
                maxY = 0;
            }

            double x = translateTransform.X;
            double y = translateTransform.Y;

            // 夹取（Clamp）
            x = Math.Max(minX, Math.Min(x, maxX));
            y = Math.Max(minY, Math.Min(y, maxY));

            translateTransform.X = x;
            translateTransform.Y = y;
        }

        #endregion 图片缩放和平移

        /// <summary>
        /// 在文本框显示缩放倍数 & 鼠标坐标
        /// </summary>
        /// <param name="mousePosCanvas"></param>
        private void UpdateInfoText(Point mousePosCanvas)
        {
            // 缩放倍数(如 "125%")
            string zoomText = $"{(ZoomFactor * 100.0):F0}%";

            // 计算鼠标在 "图片坐标" 上的像素坐标
            //  1) 去掉 translateTransform
            //  2) 再除以 scaleTransform
            double imgPosX = (mousePosCanvas.X - translateTransform.X) / scaleTransform.ScaleX;
            double imgPosY = (mousePosCanvas.Y - translateTransform.Y) / scaleTransform.ScaleY;
            CurrentPoint = new PointF((float)imgPosX, (float)imgPosY);
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

        /// <summary>
        /// 调整图片以适应窗口
        /// </summary>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        private void AdjustImageToFit(double imageWidth, double imageHeight)
        {
            double canvasWidth = dock.ActualWidth;
            double canvasHeight = dock.ActualHeight;

            // 计算缩放因子，确保图片在 Canvas 内完全显示
            double scaleX = canvasWidth / imageWidth;
            double scaleY = canvasHeight / imageHeight;
            double scale = Math.Min(scaleX, scaleY);
            ZoomFactor = scale;
            PicFactor = scale;
            // 应用缩放
            scaleTransform.ScaleX = scale;
            scaleTransform.ScaleY = scale;

            // 居中图片
            translateTransform.X = (canvasWidth - imageWidth * scale) / 2;
            translateTransform.Y = (canvasHeight - imageHeight * scale) / 2;
        }

        public WriteableBitmap writeableBitmap;
        public BitmapImage bitmapImage;
        public byte[] pixelData;
        private int imageWidth, imageHeight;
        private int currentRow = 0;
        private DispatcherTimer timer;

        //动态仿真加载图片
        private void LoadImageWithEffect(string filePath)
        {
            // 加载图片并确保像素数据加载完成
            bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(filePath, UriKind.Absolute);
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad; // 确保加载完成
            bitmapImage.EndInit();

            // 获取图片尺寸
            imageWidth = bitmapImage.PixelWidth;
            imageHeight = bitmapImage.PixelHeight;

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
            MyImage.Source = writeableBitmap;
            AdjustImageToFit(imageWidth, imageHeight);

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

        public void ClearImage()
        {
            writeableBitmap = null;
            pixelData = null;
            bitmapImage = null;
            MyImage.Source = null;
            timer = null;
        }

        private void LoadImageStatic(string filePath)
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
            MyImage.Source = writeableBitmap;
            AdjustImageToFit(imageWidth, imageHeight);
            Int32Rect rect = new Int32Rect(0, 0, imageWidth, imageHeight); // 定义一行的矩形区域
            int stride = imageWidth * 4; // 每行的字节数
            writeableBitmap.WritePixels(rect, pixelData, stride, 0);
        }

        public void LoadFile(string imagePath)
        {
            LoadImageStatic(imagePath); ;
        }

        // 添加矩形
        public void AddRectangle(double x, double y, double w, double h, Brush stroke)
        {
            Rectangle rect = new Rectangle
            {
                Width = w,
                Height = h,
                Stroke = stroke,
                StrokeThickness = 1 / ZoomFactor,
                Fill = Brushes.Transparent // 或者用部分透明更好看
            };
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);

            // 加入到与图片同级的 Canvas
            canvasContainer.Children.Add(rect);
        }

        // 在某个事件或方法里测试一下
        public void TestAddRectangles()
        {
            // 比如添加两个矩形
            AddRectangle(100.5, 100.5, 200, 150, Brushes.Red);
            AddRectangle(400, 200, 100, 300, Brushes.Blue);
        }

        // 右键矩形选择
        private bool _isRectSelecting = false;  // 是否正在右键拖拽画矩形


        private void MyCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isRectSelecting = true;
            _selector = new RectSelector(canvasContainer) { ZoomFactor = ZoomFactor / 1.0 };
            var pt = e.GetPosition(canvasContainer);
            _selector.Start(pt);
        }

        private RectSelector _selector;

        private void MyCanvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_selector.IsSelecting) return;
            var pt = e.GetPosition(canvasContainer);
            _selector.Finish(pt, ZoomFactor);
            _isRectSelecting = false;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (currentRow >= imageHeight)
            {
                timer.Stop(); // 图片加载完成，停止定时器
                return;
            }
            UpdataImage();
        }

        /// <summary>
        /// 图像的更新，减少内存的占用比例
        /// </summary>
        public void UpdataImage()
        {
            int stride = imageWidth * 4; // 每行的字节数
            int rowStartIndex = currentRow * stride;

            //锁定图片，避免UI线程冲突
            writeableBitmap.Lock();

            //计算当前行再backbuffer中的偏移量
            IntPtr pBackBuffer = writeableBitmap.BackBuffer;
            pBackBuffer += currentRow * writeableBitmap.BackBufferStride;

            //
            Marshal.Copy(pixelData, rowStartIndex, pBackBuffer, stride);

            // 仅更新当前行
            Int32Rect rect = new Int32Rect(0, currentRow, imageWidth, 1); // 定义一行的矩形区域
            writeableBitmap.AddDirtyRect(rect);

            writeableBitmap.Unlock();
            // 更新到下一行
            currentRow++;
        }
    }
}