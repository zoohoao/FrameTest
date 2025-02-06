using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageDoing.Models
{
    public class ImageDocument
    {
        public WriteableBitmap ImageBitmap { get; set; }

        public double ZoomFactor { get; set; } = 1.0;
        public double OffsetX { get; set; }
        public double OffsetY { get; set; }
    }
}