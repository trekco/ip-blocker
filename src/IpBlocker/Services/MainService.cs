using FluentScheduler;

using Microsoft.Extensions.DependencyInjection;

using Trekco.IpBlocker.SqlLite.Core;

namespace Trekco.IpBlocker.Services
{
    internal class MainService
    {
        private readonly IServiceScopeFactory scopeFactory;

        public MainService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        public void Start()
        {
            InitDatabase();

            InitScheduler();
        }

        private void InitScheduler()
        {
            var registry = new Registry();

            registry.NonReentrantAsDefault();

            registry.Schedule(() =>
                {
                    using (var scope = scopeFactory.CreateScope())
                    {
                        var service = scope.ServiceProvider.GetRequiredService<BlockService>();
                        service.Execute();
                    }
                })
                .AndThen(() =>
                {
                    using (var scope = scopeFactory.CreateScope())
                    {
                        var service = scope.ServiceProvider.GetRequiredService<UnBlockService>();
                        service.Execute();
                    }
                })
                .ToRunEvery(30)
                .Seconds();

            JobManager.JobException += JobManager_JobException;
            JobManager.Initialize(registry);
        }

        private void InitDatabase()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<IpBlockerDatabase>();
                DbInitializer.Initialize(dbContext);
            }
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