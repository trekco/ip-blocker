using FluentScheduler;

using Trekco.IpBlocker.Core.Interfaces;

namespace Trekco.IpBlocker.Services
{
    public class UnBlockService : IJob
    {
        private readonly IDataStore dataStore;
        private readonly IFirewallIpBlocker ipBlocker;

        public UnBlockService(IDataStore dataStore, IFirewallIpBlocker ipBlocker)
        {
            this.dataStore = dataStore;
            this.ipBlocker = ipBlocker;
        }

        public void Execute()
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