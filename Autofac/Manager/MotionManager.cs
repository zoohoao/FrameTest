using Autofac.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autofac
{
    public class MotionManager : IMotionManager
    {
        public void Move()
        {
            Console.WriteLine("Move");
        }
    }
}