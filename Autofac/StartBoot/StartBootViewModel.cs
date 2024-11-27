using Autofac.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Autofac.ViewModel
{
    public class StartBootViewModel
    {
        private readonly IAcquisitionManager _acquisitionManager;
        private readonly IReportManager _reportManager;

        public StartBootViewModel(IAcquisitionManager acquisitionManager, IReportManager reportManager)
        {
            _acquisitionManager = acquisitionManager;
            _reportManager = reportManager;
            StartSystemCommand = new RelayCommand(StartSystem);
        }

        public ICommand StartSystemCommand { get; }

        public void StartSystem()
        {
            MessageBox.Show("IOC DI.");
            Console.WriteLine("System starting...");
            _acquisitionManager.CollectData();
            _reportManager.GenerateReport();
        }
    }
}