using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ImageDoing.Shapes
{
    public class CustomEllipse : FrameworkElement
    {
        private readonly Ellipse _ellipse; // 内部的原生 Ellipse
        public string ShapeId { get; set; } // 自定义属性：形状 ID
        public string Name { get; set; } // 自定义属性：形状名称
        public bool IsSelectable { get; set; } = true; // 自定义属性：是否可选择
        public object Metadata { get; set; } // 自定义属性：元数据

        public CustomEllipse(ShapeProperties properties, double zoomFactor)
        {
            _ellipse = new Ellipse
            {
                Stroke = properties.Stroke,
                Fill = properties.Fill,
                StrokeThickness = properties.StrokeThickness / zoomFactor,
                StrokeDashArray = properties.StrokeDashArray,
                Opacity = properties.Opacity
            };

            // 将 Ellipse 添加到 Visual 树
            AddVisualChild(_ellipse);
            AddLogicalChild(_ellipse);

            // 初始化自定义属性
            ShapeId = Guid.NewGuid().ToString();
            Name = "Ellipse";
        }

        // 设置位置和大小
        public void SetPosition(double x, double y, double width, double height)
        {
            Canvas.SetLeft(this, x);
            Canvas.SetTop(this, y);
            _ellipse.Width = width * 2;
            _ellipse.Height = height * 2;
            InvalidateVisual();
        }

        // 更新属性
        public void UpdateProperties(ShapeProperties properties, double zoomFactor)
        {
            _ellipse.Stroke = properties.Stroke;
            _ellipse.Fill = properties.Fill;
            _ellipse.StrokeThickness = properties.StrokeThickness / zoomFactor;
            _ellipse.StrokeDashArray = properties.StrokeDashArray;
            _ellipse.Opacity = properties.Opacity;
            InvalidateVisual();
        }

        // 自定义事件（例如点击选择）
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (IsSelectable)
            {
                // 示例：触发选择事件
                Console.WriteLine($"Selected CustomEllipse: {ShapeId}, Name: {Name}");
            }
        }

        // 实现 Visual 树
        protected override Visual GetVisualChild(int index) => _ellipse;

        protected override int VisualChildrenCount => 1;

        // 渲染逻辑
        protected override void OnRender(DrawingContext drawingContext)
        {
            // 确保 Ellipse 的尺寸和位置正确
            _ellipse.RenderTransform = new TranslateTransform
            {
                X = -_ellipse.Width / 2,
                Y = -_ellipse.Height / 2
            };
        }

        // 测量和排列
        protected override Size MeasureOverride(Size availableSize)
        {
            _ellipse.Measure(availableSize);
            return _ellipse.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _ellipse.Arrange(new Rect(finalSize));
            return finalSize;
        }
    }
}