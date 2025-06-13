using System.Windows;
using System.Windows.Media;

namespace ImageDoing.Shapes
{
    public class ShapeProperties
    {
        public Brush Stroke { get; set; } = Brushes.Black;
        public Brush Fill { get; set; } = Brushes.Transparent;
        public double StrokeThickness { get; set; } = 1.0;
        public DoubleCollection StrokeDashArray { get; set; } = null;
        public double Opacity { get; set; } = 1.0;

        public ShapeProperties Clone()
        {
            return new ShapeProperties
            {
                Stroke = Stroke,
                Fill = Fill,
                StrokeThickness = StrokeThickness,
                StrokeDashArray = StrokeDashArray != null ? new DoubleCollection(StrokeDashArray) : null,
                Opacity = Opacity
            };
        }
    }
}