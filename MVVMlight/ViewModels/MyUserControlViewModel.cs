using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVMlight.ViewModels
{
    public class MyUserControlViewModel : ViewModelBase
    {
        private string _someProperty;

        public string SomeProperty
        {
            get => _someProperty;
            set => Set(ref _someProperty, value);
        }

        public MyUserControlViewModel()
        {
            SomeProperty = "Hello from UserControl";
            UpdateTextCommand = new RelayCommand(UpdateText);
        }

        public ICommand UpdateTextCommand { get; }

        private void UpdateText()
        {
            SomeProperty = "You clicked the button!";
        }
    }
}