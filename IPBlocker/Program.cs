using System;
using System.Linq;

using IpBlocker.Core;
using IpBlocker.Core.Interfaces;
using IpBlocker.Core.Objects;
using IpBlocker.SqlLite.Core;
using IpBlocker.WindowsFirewall;

using IpBLocker.MailEnable;

namespace IpBlocker
{
    internal class Program
    {
        static IDataStore dataStore;
        private static void Main(string[] args)
        {

            dataStore  = new SqlLiteDateStore();            

            try
            {
                BlockIps();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }
                

        private static void BlockIps()
        {
            var from = new DateTime(2018, 11, 24, 0, 0, 0);

            ILogReader logFileDataSource = new MailEnableActivityLogReader();
            IFirewallIpBlocker ipBlocker = new WindowsFirewallIpBlocker();
           
            var blockPolicyFactory = new IPBlockPolicyFactory();

            var badIps = logFileDataSource.GetBadIps(from);

            foreach (var ipEntry in badIps)
            {
                var isBlocked = dataStore.IsIPBlocked(ipEntry.IP, logFileDataSource.GetName(), ipEntry.Port, ipEntry.Protocol.ToString());

                if (!isBlocked)
                {
                    var blockEntry = new BlockedEntry(ipEntry);
                    var policy = blockPolicyFactory.GetPolicy(blockEntry, logFileDataSource.GetName());
                   
                    if (policy.ShouldBlock() && ipBlocker.Block(blockEntry, policy, out var ruleName))
                    {
                        blockEntry.RuleName = ruleName;
                        blockEntry.IsBLocked = true;
                        dataStore.Add(blockEntry, policy, logFileDataSource.GetName());
                    }
                }
            }
        }
    }
}