using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ImageDoing.Models
{
    // RectSelector.cs —   负责管理“画矩形”状态机
    public class RectSelector
    {
        private readonly Canvas _canvas;
        private Rectangle _tempRect;
        private Point _startPos;

        public bool IsSelecting { get; private set; }
        public double ZoomFactor { get; set; } = 1.0;

        public RectSelector(Canvas canvas)
        {
            _canvas = canvas;
        }

        public void Start(Point pos)
        {
            if (IsSelecting) return;
            IsSelecting = true;
            _startPos = pos;
            _tempRect = new Rectangle
            {
                Stroke = Brushes.Blue,
                StrokeThickness = 1 / ZoomFactor,
                Fill = new SolidColorBrush(Color.FromArgb(40, 0, 0, 255))
            };
            Canvas.SetLeft(_tempRect, pos.X);
            Canvas.SetTop(_tempRect, pos.Y);
            _tempRect.Width = _tempRect.Height = 0;

            _canvas.Children.Add(_tempRect);
            _canvas.CaptureMouse();
        }

        public void Update(Point pos, double zf)
        {
            if (!IsSelecting) return;
            var x = Math.Min(_startPos.X, pos.X);
            var y = Math.Min(_startPos.Y, pos.Y);
            var w = Math.Abs(pos.X - _startPos.X);
            var h = Math.Abs(pos.Y - _startPos.Y);

            Canvas.SetLeft(_tempRect, x);
            Canvas.SetTop(_tempRect, y);
            _tempRect.Width = w;
            _tempRect.Height = h;
            // _tempRect.StrokeThickness = 1 / zf;
        }

        public void Finish(Point pos, double zf)
        {
            if (!IsSelecting) return;
            IsSelecting = false;
            _canvas.ReleaseMouseCapture();

            // 最终矩形
            var x = Math.Min(_startPos.X, pos.X);
            var y = Math.Min(_startPos.Y, pos.Y);
            var w = Math.Abs(pos.X - _startPos.X);
            var h = Math.Abs(pos.Y - _startPos.Y);

            // 如果需要保留临时框就克隆；否则直接用 _tempRect
            if (w > 1 && h > 1)
            {
                var finalRect = new Rectangle
                {
                    Width = w,
                    Height = h,
                    Stroke = Brushes.Red,
                    StrokeThickness = 1 / ZoomFactor
                };
                Canvas.SetLeft(finalRect, x);
                Canvas.SetTop(finalRect, y);
                _canvas.Children.Add(finalRect);
            }

            // 移除/隐藏临时框
            _canvas.Children.Remove(_tempRect);
            _tempRect = null;
        }
    }
}