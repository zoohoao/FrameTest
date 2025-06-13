using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace ImageDoing.Shapes
{
    public class EllipseShap : ZShapes
    {
        public Ellipse ellipse { get; set; }

        public EllipseShap(Canvas _canvas) : base(_canvas)
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
                var finalRect = new Ellipse
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
            Canvas.Children.Remove(ellipse);
            ellipse = null;
        }

        public override void Start(Point point)
        {
            _startPos = point;
            ellipse = new Ellipse
            {
                Stroke = Brushes.Blue,
                StrokeThickness = 1 / ZoomFactor,
                Fill = new SolidColorBrush(Color.FromArgb(40, 0, 0, 255))
            };
            Canvas.SetLeft(ellipse, point.X);
            Canvas.SetTop(ellipse, point.Y);
            ellipse.Width = ellipse.Height = 0;

            Canvas.Children.Add(ellipse);
            Canvas.CaptureMouse();
        }

        public override void Update(Point point)
        {
            var x = Math.Min(_startPos.X, point.X);
            var y = Math.Min(_startPos.Y, point.Y);
            var w = Math.Abs(point.X - _startPos.X);
            var h = Math.Abs(point.Y - _startPos.Y);

            Canvas.SetLeft(ellipse, x);
            Canvas.SetTop(ellipse, y);
            ellipse.Width = w;
            ellipse.Height = h;
        }
    }
}