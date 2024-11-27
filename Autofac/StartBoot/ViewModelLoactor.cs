using Autofac.ManagerModule;
using Autofac.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autofac.StartBoot
{
    internal class ViewModelLoactor
    {
        private IContainer _container;
        public StartBootViewModel StartBootMode { get; }

        public ViewModelLoactor()
        {
            var builder = new ContainerBuilder();

            // 注册组件
            builder.RegisterModule<AcquisitionManagerModule>();
            builder.RegisterModule<MotionManagerModule>();
            builder.RegisterModule<PulseManagerModule>();
            builder.RegisterModule<ReportManagerModule>();
            //// 注册 ViewModel
            builder.RegisterType<StartBootViewModel>();
            _container = builder.Build();

            StartBootMode = _container.Resolve<StartBootViewModel>();
        }
    }
}