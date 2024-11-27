using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MVVMlight.Messender;
using System;
using System.Windows;
using System.Windows.Input;

namespace MVVMlight.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        private string _text;

        public string Text
        {
            get => _text;
            set => Set(ref _text, value); // 自动触发 PropertyChanged 事件
        }

        public ICommand UpdateTextCommand { get; }

        public MainViewModel()
        {
            //注册消息传递事件
            Messenger.Default.Register<SettingsUpdatedMessage>(this, OnSettingsUpdated);

            Text = "Hello, MVVM Light!";
            UpdateTextCommand = new RelayCommand(UpdateText);
        }

        /// <summary>
        /// 激活消息来源
        /// </summary>
        /// <param name="message"></param>
        private void OnSettingsUpdated(SettingsUpdatedMessage message)
        {
            // 处理更新
            MessageBox.Show($"Settings updated: {message.UpdatedSetting}");
        }

        private void UpdateText()
        {
            Text = "You clicked the button!";
        }
    }
}