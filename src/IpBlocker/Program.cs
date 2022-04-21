using IpBLocker.MailEnable;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Topshelf;

using Trekco.IpBlocker.Core;
using Trekco.IpBlocker.Core.Interfaces;
using Trekco.IpBlocker.IpLocation.Maxmind;
using Trekco.IpBlocker.Services;
using Trekco.IpBlocker.SqlLite.Core;
using Trekco.IpBlocker.WindowsFirewall;

namespace Trekco.IpBlocker
{
    internal class Program
    {
        private static readonly IServiceProvider ServiceProvider;

        static Program()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .AddEnvironmentVariables()
                .Build();

            var services = new ServiceCollection();

            services.AddDbContext<IpBlockerDatabase>(opt =>
                opt.UseSqlite(configuration.GetConnectionString("IpBlockerDatabase")));

            services.AddSingleton<IConfiguration>(sp => configuration);
            services.AddSingleton<MainService>();
            services.AddSingleton<IIPLocator>(sp =>
            {
                var maxmind = new MaxmindIpLocation(sp.GetRequiredService<IConfiguration>());
                maxmind.Initialize();

                return maxmind;
            });

            services.AddScoped<IDataStore, SqlLiteDateStore>();
            services.AddScoped<IIPBlockPolicyFactory, IPBlockPolicyFactory>();
            services.AddScoped<ILogReader, MailEnableActivityLogReader>();
            services.AddScoped<IFirewallIpBlocker, WindowsFirewallIpBlocker>();
            services.AddScoped<BlockService>();
            services.AddScoped<UnBlockService>();

            ServiceProvider = services.BuildServiceProvider();
        }

        private static void Main(string[] args)
        {
            var rc = HostFactory.Run(x =>
            {
                x.Service<MainService>(s =>
                {
                    s.ConstructUsing(name => ServiceProvider.GetRequiredService<MainService>());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("IpBlocker Service");
                x.SetDisplayName("IpBlocker Service");
                x.SetServiceName("IpBlocker");
            });

            var exitCode = (int) Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }
}