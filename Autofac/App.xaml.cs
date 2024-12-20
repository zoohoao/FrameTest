﻿using Autofac.Interface;
using Autofac.Manager;
using Autofac.ManagerModule;
using Autofac.StartBoot;
using Autofac.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Autofac
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            try
            {
                var StartBoot = new StartBootView();
                StartBoot.DataContext = new ViewModelLoactor().StartBootMode;
                StartBoot.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}