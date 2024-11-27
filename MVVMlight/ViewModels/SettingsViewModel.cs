using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MVVMlight.Messender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVMlight.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel()
        {
            UpdateTextCommand1 = new RelayCommand(UpdateSettings);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        public void UpdateSettings()
        {
            // 假设用户更新了某个设置
            string newSetting = "DarkMode";

            // 发送消息
            Messenger.Default.Send(new SettingsUpdatedMessage { UpdatedSetting = newSetting });
        }

        public ICommand UpdateTextCommand1 { get; }
    }
}