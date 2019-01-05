using FluentScheduler;
using IpBlocker.Interfaces;
using SimpleInjector;
using System;

namespace IpBlocker.Services
{
    internal class MainService
    {
        private readonly Container container;

        public MainService(Container container)
        {
            this.container = container;
        }

        public void Start()
        {
            
           
            var registry = new Registry();

            registry.NonReentrantAsDefault();

            registry.Schedule(() => container.GetInstance<IBlockService>())
                    .AndThen(() => container.GetInstance<IUnBlockService>())                  
                    .ToRunEvery(30)
                    .Seconds();


            JobManager.JobException += JobManager_JobException;
            JobManager.Initialize(registry);

        }

        private void JobManager_JobException(JobExceptionInfo obj)
        {
            Console.WriteLine(obj.Exception.Message);
        }

        public void Stop()
        {

            JobManager.StopAndBlock();
        }
    }
}