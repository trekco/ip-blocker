using IpBlocker.Core;
using IpBlocker.Core.Interfaces;
using IpBlocker.Interfaces;
using IpBlocker.IpLocation.Maxmind;
using IpBlocker.Services;
using IpBlocker.SqlLite.Core;
using IpBlocker.WindowsFirewall;
using IpBLocker.MailEnable;
using SimpleInjector;
using System;
using Topshelf;

namespace IpBlocker
{
    internal class Program
    {
        private static readonly Container container;

        static Program()
        {
            container = new Container();

            container.RegisterSingleton<IIPLocator>(() =>
            {
                var mm = new MaxmindIpLocation();
                mm.Initialize();
                return mm;
            });

            container.Register<IIPBlockPolicyFactory, IPBlockPolicyFactory>();
            container.RegisterSingleton<IDataStore, SqlLiteDateStore>();
            container.Register<ILogReader, MailEnableActivityLogReader>();
            container.Register<IFirewallIpBlocker, WindowsFirewallIpBlocker>();
            container.Register<IBlockService, BlockService>();
            container.Register<IUnBlockService, UnBlockService>();

            container.Verify();
        }

        private static void Main(string[] args)
        {
            var rc = HostFactory.Run(x =>
            {
                x.Service<MainService>(s =>
                {
                    s.ConstructUsing(name => new MainService(container));
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("IpBlocker Service");
                x.SetDisplayName("IpBlocker Service");
                x.SetServiceName("IpBlocker");
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }
}