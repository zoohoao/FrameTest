using ImageDoing.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDoing.Services
{
    public class PanZoomService
    {
        private ImageDocument _doc;

        public PanZoomService(ImageDocument doc)
        {
            _doc = doc;
        }

        public void ZoomAt(Point centerPos, double factor)
        {
            double beforeX = (centerPos.X - _doc.OffsetX) / _doc.ZoomFactor;
            double beforeY = (centerPos.Y - _doc.OffsetY) / _doc.ZoomFactor;

            _doc.ZoomFactor *= factor;
            if (_doc.ZoomFactor < 0.05) _doc.ZoomFactor = 0.05;
            if (_doc.ZoomFactor > 64) _doc.ZoomFactor = 64;

            double afterX = (centerPos.X - _doc.OffsetX) / _doc.ZoomFactor;
            double afterY = (centerPos.Y - _doc.OffsetY) / _doc.ZoomFactor;

            double diffX = afterX - beforeX;
            double diffY = afterY - beforeY;

            _doc.OffsetX += diffX * _doc.ZoomFactor;
            _doc.OffsetY += diffY * _doc.ZoomFactor;
        }

        public void Pan(double dx, double dy)
        {
            _doc.OffsetX += dx;
            _doc.OffsetY += dy;
        }

        public void ClamToBoundary(double canvasWidth, double canvasHeight)
        {
        }
    }
}