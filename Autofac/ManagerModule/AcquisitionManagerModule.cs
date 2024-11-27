using Autofac.Interface;
using Autofac.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autofac.ManagerModule
{
    internal class AcquisitionManagerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AcquisitionManager>().As<IAcquisitionManager>().SingleInstance();
        }
    }
}