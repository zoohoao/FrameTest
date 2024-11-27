using Autofac.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autofac.ManagerModule
{
    internal class MotionManagerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MotionManager>().As<IMotionManager>().SingleInstance();
        }
    }
}