using IpBlocker.Core.Interfaces;
using IpBlocker.Interfaces;
using System;

namespace IpBlocker.Services
{
    public class UnBlockService : IUnBlockService
    {
        private readonly IDataStore dataStore;
        private readonly IFirewallIpBlocker ipBlocker;

        public UnBlockService(IDataStore dataStore, IFirewallIpBlocker ipBlocker)
        {
            this.dataStore = dataStore;
            this.ipBlocker = ipBlocker;

            Run();
        }

        public void Run()
        {
            Console.WriteLine($"[{DateTime.Now}] {nameof(UnBlockService)} - Starting");

            var entriesToUnblock = dataStore.GetIpsToUnblock(DateTime.Now);

            foreach (var entry in entriesToUnblock)
            {
                if (ipBlocker.Unblock(entry))
                {
                    dataStore.UnblockBlockedIpRecord(entry);
                }
            }

            Console.WriteLine($"[{DateTime.Now}] {nameof(UnBlockService)} - Complete");
        }
    }
}