using IpBlocker.Core.Objects;
using System;

namespace IpBlocker.Core
{
    public class IPBlockPolicyFactory : IIPBlockPolicyFactory
    {
        public IPBlockPolicy GetPolicy(BlockedEntry blockEntry)
        {

            var shouldBlock = !blockEntry.IpLocation.ToLower().Contains("south africa");
            var blockTime = TimeSpan.FromHours(24);

            if(blockEntry.IpLocation.ToLower().Contains("south africa"))
            {
                blockTime = TimeSpan.FromSeconds(0);
            }
            else if (blockEntry.IpLocation.ToLower().Contains("Unknown"))
            {
                blockTime = TimeSpan.FromMinutes(60);
            }

            if(blockEntry.Ip == "127.0.0.1")
            {
                blockTime = TimeSpan.FromSeconds(0);
                shouldBlock = false;
            }

            return new IPBlockPolicy(shouldBlock, DateTime.Now.Add(blockTime), $"(IP-BLOCK) {blockEntry.Source} Blocked IP");
        }
    }
}