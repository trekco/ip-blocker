using Microsoft.Extensions.Configuration;

using Trekco.IpBlocker.Core.Extensions;
using Trekco.IpBlocker.Core.Objects;

namespace Trekco.IpBlocker.Core
{
    public class IPBlockPolicyFactory : IIPBlockPolicyFactory
    {
        private readonly IConfiguration configuration;

        public IPBlockPolicyFactory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IPBlockPolicy GetPolicy(BlockedEntry blockEntry)
        {
            var whiteListedLocations = configuration.GetSection("IPBlockPolicyFactory:WhiteListedLocations").Value
                .ToStringArray();
            var whiteListedIps = configuration.GetSection("IPBlockPolicyFactory:WhiteListedIps").Value.ToStringArray();

            var ip = blockEntry.IpLocation.Trim().ToLower();
            var shouldBlock = !whiteListedLocations.Any(l => ip.Contains(l.ToLower()));
            var blockTime = TimeSpan.FromHours(24);

            if (!shouldBlock)
            {
                blockTime = TimeSpan.FromSeconds(0);
            }
            else if (shouldBlock && blockEntry.IpLocation.ToLower().Contains("Unknown"))
            {
                blockTime = TimeSpan.FromMinutes(60);
            }

            if (whiteListedIps.Any(ip => ip.Equals(blockEntry.Ip)))
            {
                blockTime = TimeSpan.FromSeconds(0);
                shouldBlock = false;
            }

            return new IPBlockPolicy(shouldBlock, DateTime.Now.Add(blockTime),
                $"(IP-BLOCK) {blockEntry.Source} Blocked IP");
        }
    }
}