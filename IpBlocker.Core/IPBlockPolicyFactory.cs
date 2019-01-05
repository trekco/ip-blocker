using IpBlocker.Core.Objects;
using System;
using System.Configuration;
using IpBlocker.Core.Extensions;
using System.Linq;

namespace IpBlocker.Core
{
    public class IPBlockPolicyFactory : IIPBlockPolicyFactory
    {
        public IPBlockPolicy GetPolicy(BlockedEntry blockEntry)
        {

            var whiteListedLocations = ConfigurationManager.AppSettings["IPBlockPolicyFactory:WhiteListedLocations"].ToArray();
            var whiteListedIps = ConfigurationManager.AppSettings["IPBlockPolicyFactory:WhiteListedIps"].ToArray();
                                 
            var shouldBlock = !whiteListedLocations.Any(l => blockEntry.IpLocation.Trim().ToLower().Contains(l.ToLower()));
            var blockTime = TimeSpan.FromHours(24);

            if(!shouldBlock)
            {
                blockTime = TimeSpan.FromSeconds(0);
            }
            else if (shouldBlock && blockEntry.IpLocation.ToLower().Contains("Unknown"))
            {
                blockTime = TimeSpan.FromMinutes(60);
            }

            if(whiteListedIps.Any(ip => ip.Equals(blockEntry.Ip)))
            {
                blockTime = TimeSpan.FromSeconds(0);
                shouldBlock = false;
            }

            return new IPBlockPolicy(shouldBlock, DateTime.Now.Add(blockTime), $"(IP-BLOCK) {blockEntry.Source} Blocked IP");
        }
    }
}