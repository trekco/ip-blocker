﻿using FluentScheduler;

using Trekco.IpBlocker.Core;
using Trekco.IpBlocker.Core.Interfaces;
using Trekco.IpBlocker.Core.Objects;

namespace Trekco.IpBlocker.Services
{
    public class BlockService : IJob
    {
        private readonly IDataStore dataStore;
        private readonly IFirewallIpBlocker ipBlocker;
        private readonly IIPLocator ipLocator;
        private readonly ILogReader logFileDataSource;
        private readonly IIPBlockPolicyFactory policyFactory;

        public BlockService(IDataStore dataStore,
            ILogReader logFileDataSource,
            IIPLocator ipLocator,
            IFirewallIpBlocker ipBlocker,
            IIPBlockPolicyFactory policyFactory)
        {
            this.dataStore = dataStore;
            this.logFileDataSource = logFileDataSource;
            this.ipLocator = ipLocator;
            this.ipBlocker = ipBlocker;
            this.policyFactory = policyFactory;
        }

        public void Execute()
        {
            var fromDate = GetLastRunDate();

            if (fromDate.Date != DateTime.Now.Date)
            {
                fromDate = DateTime.Today;
            }

            Console.WriteLine($"[{DateTime.Now}] {nameof(BlockService)} - Starting from {fromDate}");

            var badIps = logFileDataSource.GetBadIps(fromDate);
            var runtime = badIps.LastOrDefault()?.Time.AddSeconds(1) ?? DateTime.Now;

            foreach (var ipEntry in badIps)
            {
                var isBlocked = dataStore.IsIPBlocked(ipEntry.IP, logFileDataSource.GetName(), ipEntry.Ports,
                    ipEntry.Protocol.ToString());

                if (!isBlocked)
                {
                    var blockEntry = new BlockedEntry(ipEntry, logFileDataSource.GetName());
                    blockEntry.IpLocation = ipLocator.GetIpLocation(blockEntry.Ip);

                    var policy = policyFactory.GetPolicy(blockEntry);

                    if (policy.ShouldBlock() && ipBlocker.Block(blockEntry, policy, out var ruleName))
                    {
                        blockEntry.RuleName = ruleName;
                        blockEntry.IsBLocked = true;
                        dataStore.Add(blockEntry, policy, logFileDataSource.GetName());
                    }
                }
            }

            dataStore.SaveConfigValue("LastRunDate", runtime.ToString());

            Console.WriteLine($"[{DateTime.Now}] {nameof(BlockService)} - Complete");
        }

        private DateTime GetLastRunDate()
        {
            var value = dataStore.GetConfigValue("LastRunDate");

            if (value != null)
            {
                return DateTime.Parse(value);
            }

            return DateTime.Today;
        }
    }
}