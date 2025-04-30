using ImageDoing.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace ImageDoing
{
    public class ShapsFactor
    {
        public static ZShapes Shape { get; set; }

        public static ZShapes CreateShaps(ShapesEnum shapeModel, Canvas canvas)
        {
            switch (shapeModel)
            {
                case ShapesEnum.RectangleShap:
                    Shape = new RectangleShap(canvas);
                    break;

                case ShapesEnum.EllipseShap:
                    Shape = new EllipseShap(canvas);
                    break;

                case ShapesEnum.polygonShap:
                    Shape = new polygonShap(canvas);
                    break;

                    //case DrawMode.ArrowLine:
                    //    Shape = new ArrowLineShape();
                    //    shapes.Add(Shape); // 提前加入以显示实时预览
                    //    break;
            }
            return Shape;
        }
    }
}