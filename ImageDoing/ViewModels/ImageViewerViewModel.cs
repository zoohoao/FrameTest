using ImageDoing.Models;
using ImageDoing.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ImageDoing.ViewModels
{
    public class ImageViewerViewModel : INotifyPropertyChanged
    {
        private ImageDocument _doc;
        private PanZoomService _panZoomService;

        public ImageViewerViewModel()
        {
            _doc = new ImageDocument();
            _panZoomService = new PanZoomService(_doc);
        }

        public WriteableBitmap ImageBitmap
        {
            get => _doc.ImageBitmap;
            set
            {
                _doc.ImageBitmap = value;
                OnPropertyChanged();
            }
        }

        public double ZoomFactor => _doc.ZoomFactor;
        public double OffsetX => _doc.OffsetX;
        public double OffsetY => _doc.OffsetY;
        public ICommand ZoomInCommand { get; }
        public ICommand ZoomOutCommand { get; }

        public void LoadImage(string Path)
        {
            var bmp = new BitmapImage(new Uri(Path, UriKind.Absolute));
            var wbmp = new WriteableBitmap(bmp);
            ImageBitmap = wbmp;
        }

        private void ZoomIn()
        {
            _panZoomService.ZoomAt(new System.Drawing.Point(0, 0), 1.2);
            OnPropertyChanged(nameof(ZoomFactor));
            OnPropertyChanged(nameof(OffsetX));
            OnPropertyChanged(nameof(OffsetY));
        }

        private void ZoomOut()
        {
            _panZoomService.ZoomAt(new System.Drawing.Point(0, 0), 0.8);
            OnPropertyChanged(nameof(ZoomFactor));
            OnPropertyChanged(nameof(OffsetX));
            OnPropertyChanged(nameof(OffsetY));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class RelayCommand : ICommand
    {
        private Action<object> _execute;
        private Func<object, bool> _canExecute;

        public RelayCommand(Action<object> exec, Func<object, bool> can = null)
        {
            _execute = exec;
            _canExecute = can;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}