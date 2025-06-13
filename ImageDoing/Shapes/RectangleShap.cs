using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace ImageDoing.Shapes
{
    public class RectangleShap : ZShapes
    {
        public Rectangle rectangle;

        public RectangleShap(Canvas _canvas) : base(_canvas)
        {
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }

        public override void Finish(Point point, double zf)
        {
            Canvas.ReleaseMouseCapture();

            // 最终矩形
            var x = Math.Min(_startPos.X, _startPos.X);
            var y = Math.Min(_startPos.Y, _startPos.Y);
            var w = Math.Abs(point.X - _startPos.X);
            var h = Math.Abs(point.Y - _startPos.Y);

            // 如果需要保留临时框就克隆；否则直接用 _tempRect
            if (w > 1 && h > 1)
            {
                var finalRect = new Rectangle
                {
                    Width = w,
                    Height = h,
                    Stroke = Brushes.Red,
                    StrokeThickness = 1 / zf
                };
                Canvas.SetLeft(finalRect, x);
                Canvas.SetTop(finalRect, y);
                Canvas.Children.Add(finalRect);
            }

            // 移除/隐藏临时框
            Canvas.Children.Remove(rectangle);
            rectangle = null;
        }

        public override void Start(Point point)
        {
            _startPos = point;
            rectangle = new Rectangle
            {
                Stroke = Brushes.Blue,
                StrokeThickness = 1 / ZoomFactor,
                Fill = new SolidColorBrush(Color.FromArgb(40, 0, 0, 255))
            };
            Canvas.SetLeft(rectangle, point.X);
            Canvas.SetTop(rectangle, point.Y);
            rectangle.Width = rectangle.Height = 0;

            Canvas.Children.Add(rectangle);
            Canvas.CaptureMouse();
        }

        public override void Update(Point point)
        {
            var x = Math.Min(_startPos.X, point.X);
            var y = Math.Min(_startPos.Y, point.Y);
            var w = Math.Abs(point.X - _startPos.X);
            var h = Math.Abs(point.Y - _startPos.Y);

            Canvas.SetLeft(rectangle, x);
            Canvas.SetTop(rectangle, y);
            rectangle.Width = w;
            rectangle.Height = h;
        }
    }
}