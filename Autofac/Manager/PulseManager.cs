using Autofac.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autofac
{
    public class PulseManager : IPulseManager
    {
        public bool Open()
        {
            Console.WriteLine("Open");
            return true;
        }
    }
}