using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FluentValidation
{
    public class MainViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _name;
        private readonly IValidator<MainViewModel> _validator = new UserValidator();

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string this[string columnName]
        {
            get
            {
                var validationResult = _validator.Validate(this);
                var error = validationResult.Errors.FirstOrDefault(e => e.PropertyName == columnName);
                return error?.ErrorMessage;
            }
        }

        public ICommand MyCommand { get; set; }
        public ICommand MyCommand1 { get; set; }

        public string Error => throw new NotImplementedException();

        public MainViewModel()
        {
            MyCommand = new RelayCommand(ExecuteCommand);
            MyCommand1 = new RelayCommand(ExecuteCommand1);
        }

        private void ExecuteCommand1()
        {
            MessageBox.Show("Test");
        }

        private void ExecuteCommand()
        {
            Name = "Hello, WPF!";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}