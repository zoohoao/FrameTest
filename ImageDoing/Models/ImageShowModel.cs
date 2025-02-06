using System.Drawing;

namespace ImageDoing
{
    public class ImageShowModel
    {
        public ImageShowModel()
        {
        }

        private double zoomfactor;

        public double ZoomFactor
        {
            get { return zoomfactor; }
            set { zoomfactor = value; }
        }

        private string imagepa;

        public string ImagePath
        {
            set { imagepa = value; }
        }

        private Bitmap bitmap;

        public Bitmap CurrentBitMap

        {
            get { return bitmap; }
            set { bitmap = value; }
        }

        public PointF CurrentPoint { get; set; }
    }
}