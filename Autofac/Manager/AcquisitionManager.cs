using Autofac.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autofac.Manager
{
    internal class AcquisitionManager : IAcquisitionManager
    {
        private readonly IMotionManager _motionManager;
        private readonly IPulseManager _pulseManager;

        public AcquisitionManager(IMotionManager motionManager, IPulseManager pulseManager)
        {
            _motionManager = motionManager;
            _pulseManager = pulseManager;
        }

        public void CollectData()
        {
            Console.WriteLine("Data collection started.");
            _motionManager.Move();
            _pulseManager.Open();
        }
    }
}