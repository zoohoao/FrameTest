using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ImageDoing
{
    public abstract class ZShapes : UIElement
    {
        public Canvas Canvas { get; set; }
        protected Point _startPos;
        public double ZoomFactor { get; set; } = 1.0;

        protected ZShapes(Canvas _canvas)
        {
            Canvas = _canvas;
        }

        public abstract void Start(Point point);

        public abstract void Update(Point point);

        public abstract void Draw();

        public abstract void Finish(Point point, double zf);
    }
}