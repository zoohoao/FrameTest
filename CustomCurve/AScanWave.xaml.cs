using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace CustomCurve
{
    public partial class AScanWave : UserControl
    {
        private DispatcherTimer timer;
        private PlotModel plotModel = new PlotModel { };
        private LineSeries scatterSeries1 = new LineSeries { Color = OxyColors.Red, StrokeThickness = 1 };

        private LinearAxis dateAxis = new LinearAxis
        {
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Dot,
            IsZoomEnabled = false,
            IsPanEnabled = false,
            Title = "点数"
        };

        private readonly int maxPoints = 12000; // 总点数
        private int displayPoints = 4000; // 显示点数
        private List<double> waveValues = new List<double>(); // 存储 120,000 个点
        private int startIndex = 0; // 显示窗口的起始下标
        private int sampleInterval = 1; // 采样间隔，初始为 1
        private bool isDragging = false; // 拖动状态
        private Point lastMousePosition; // 鼠标拖动起始位置
        private Random rand = new Random(); // 用于生成噪声
        private const int minInterval = 1;
        private const int maxInterval = 10;

        public AScanWave()
        {
            InitializeComponent();
            var valueAxis = new LinearAxis
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Title = "幅度",
                Minimum = -0.5,
                Maximum = 0.5,
                IsZoomEnabled = false,
                IsPanEnabled = false
            };
            plotModel.Axes.Add(valueAxis);
            plotModel.Series.Add(scatterSeries1);
            Plot_view.Model = plotModel;
            Plot_view.IsHitTestVisible = true;
            var controller = new PlotController();
            controller.UnbindAll();
            Plot_view.Controller = controller;
            // 初始化 120,000 个点
            waveValues = new List<double>(maxPoints);
            scatterSeries1.Points.Capacity = displayPoints;
            GenerateWaveform();
            UpdatePlot();

            // 绑定鼠标事件
            Plot_view.MouseDown += Plot_MouseDown;
            Plot_view.MouseMove += Plot_MouseMove;
            Plot_view.MouseUp += Plot_MouseUp;
            Plot_view.MouseWheel += Plot_MouseWheel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Start();
        }

        private void GenerateWaveform()
        {
            waveValues.Clear();
            double tStep = 1.0 / 5_000_000; // 采样率 5MHz
            double freq = 2_000_000; // 超声波频率 2MHz
            double decay = 0.00001; // 衰减系数
            double[] echoPositions = { 1000, 5000, 15000, 30000, 60000, 90000 }; // 反射峰位置
            double[] echoAmplitudes = { 0.4, 0.3, 0.25, 0.2, 0.15, 0.1 }; // 反射峰幅度

            for (int i = 0; i < maxPoints; i++)
            {
                double t = i * tStep;
                double value = 0;
                for (int j = 0; j < echoPositions.Length; j++)
                {
                    double tEcho = echoPositions[j] * tStep;
                    double deltaT = t - tEcho;
                    if (Math.Abs(deltaT) < 5.0 / freq)
                    {
                        value += echoAmplitudes[j] * Math.Sin(2 * Math.PI * freq * deltaT) * Math.Exp(-decay * i);
                    }
                }
                value += (rand.NextDouble() - 0.5) * 0.02; // 噪声
                waveValues.Add(value);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            GenerateWaveform(); // 模拟动态数据
            UpdatePlot();
        }

        private void UpdatePlot()
        {
            scatterSeries1.Points.Clear();

            for (int i = 0; i < displayPoints; i++)
            {
                int dataIndex = startIndex + i;

                if (dataIndex >= 0 && dataIndex < maxPoints)
                {
                    scatterSeries1.Points.Add(new DataPoint(dataIndex, waveValues[dataIndex]));
                }
            }

            // 设置 X 轴的真实下标范围
            dateAxis.Minimum = startIndex;
            dateAxis.Maximum = startIndex + displayPoints - 1;

            plotModel.InvalidatePlot(true); // 刷新视图
        }

        private void Plot_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                isDragging = true;
                lastMousePosition = e.GetPosition(Plot_view);
                Plot_view.CaptureMouse();
            }
        }

        private void Plot_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentPosition = e.GetPosition(Plot_view);
                double deltaX = currentPosition.X - lastMousePosition.X;

                // 转换为数据点偏移，考虑采样间隔
                int indexDelta = (int)(deltaX * displayPoints / Plot_view.ActualWidth * sampleInterval);
                startIndex -= indexDelta; // 反向移动：左拖增加 startIndex，右拖减少

                // 限制 startIndex 范围
                startIndex = Math.Max(0, Math.Min(maxPoints - displayPoints * sampleInterval, startIndex));

                lastMousePosition = currentPosition;
                UpdatePlot(); // 立即更新视图
            }
        }

        private void Plot_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                isDragging = false;
                Plot_view.ReleaseMouseCapture();
            }
        }

        private void ShowRange(int begin, int length)
        {
            startIndex = begin;
            displayPoints = length;
            sampleInterval = 1; // 可选：每个点都显示
            UpdatePlot();
        }

        private void Plot_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var position = e.GetPosition(Plot_view).ToScreenPoint();
            var xAxis = plotModel.Axes[0];

            // 获取鼠标位置对应的X轴数据值
            double xData = xAxis.InverseTransform(position.X);

            // 缩放因子：滚轮向上放大1.1倍，向下缩小1/1.1倍
            double zoomFactor = e.Delta > 0 ? 1.1 : 1 / 1.1;

            // 使用 OxyPlot 提供的ZoomAt方法围绕鼠标数据点缩放
            xAxis.ZoomAt(zoomFactor, xData);

            // 限制缩放范围，避免过度缩放
            if (xAxis.ActualMinimum < 0)
                xAxis.Zoom(0, xAxis.ActualMaximum - xAxis.ActualMinimum);
            if (xAxis.ActualMaximum > plotModel.Series.Count)
                xAxis.Zoom(xAxis.ActualMinimum, plotModel.Series.Count);

            plotModel.InvalidatePlot(false);
            e.Handled = true;
        }
    }
}