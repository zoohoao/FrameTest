using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace ImageDoing
{
    public class ImageShowViewModel : INotifyPropertyChanged
    {
        private ImageShowModel model;

        public ImageShowModel imageShowModel
        {
            get
            {
                if (model == null)
                {
                    model = new ImageShowModel();
                }
                return model;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}