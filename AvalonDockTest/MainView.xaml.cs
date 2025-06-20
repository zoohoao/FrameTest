﻿using AvalonDock.Controls;
using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace AvalonDockTest
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            LoadLayout();
            var anchorable = dockManager.Layout.Descendents().OfType<LayoutAnchorable>()
                                .FirstOrDefault(a => a.ContentId == "Chanel1");

            if (anchorable != null)
            {
                anchorable.ToolTip = "Chanel1 Scan Image";
            }
        }

        public void SaveLayout()
        {
            var serializer = new XmlLayoutSerializer(dockManager);
            using (var stream = new StreamWriter(string.Format(@".\AvalonDock_{0}.config", "dock")))
                serializer.Serialize(stream);
        }

        public void LoadLayout()
        {
            var currentContentsList = dockManager.Layout.Descendents().OfType<LayoutContent>().Where(c => c.ContentId != null).ToArray();

            var serializer = new XmlLayoutSerializer(dockManager);
            using (var stream = new StreamReader(string.Format(@".\AvalonDock_{0}.config", "dock")))
                serializer.Deserialize(stream);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveLayout();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            LoadLayout();
        }

        private void ButtonLoadImage_Click(object sender, RoutedEventArgs e)
        {
            image1.LoadFile(@"C:\Users\Administrator\Pictures\版本图片\353\5.tiff");
        }

        private void GCClear(object sender, RoutedEventArgs e)
        {
            image1.ClearImage();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private void DockManager_LayoutUpdated(object sender, EventArgs e)
        {
            foreach (var floatingWindow in dockManager.FloatingWindows)
            {
                if (floatingWindow is LayoutAnchorableFloatingWindowControl control)
                {
                    if (control.Template == null)
                    {
                        continue;
                    }
                    // 查找模板中的 DropDownButton
                    var dropDownButton = control.Template.FindName("SinglePaneContextMenu", control) as DropDownButton;
                    var PART_PinRestore = control.Template.FindName("PART_PinClose", control) as Button;
                    if (dropDownButton != null)
                    {
                        dropDownButton.Visibility = Visibility.Collapsed;
                    }
                    if (PART_PinRestore != null)
                    {
                        // PART_PinRestore.Visibility = Visibility.Hidden;
                    }
                }
            }
        }
    }
}