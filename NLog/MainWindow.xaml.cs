using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NLog
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            a();
        }

        public void a()
        {
            Logger.Trace("This is an Error message.");
            try
            {
                int a = 0;
                var b = 2 / a;
            }
            catch (Exception ex)
            {
                Logger.Error($"This is a Trace message.{ex.Message}{ex.StackTrace}");
            }
            Logger.Debug("This is a Debug message.");
            Logger.Info("This is an Info message.");
            Logger.Warn("This is a Warn message.");
            Logger.Error("This is an Error message.");
            Logger.Fatal("This is a Fatal message.");
        }
    }
}