using Autofac.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autofac.ManagerModule
{
    internal class PulseManagerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PulseManager>().As<IPulseManager>().SingleInstance();
        }
    }
}