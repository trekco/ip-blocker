using IpBlocker.Core.Objects;
using System;

namespace IpBlocker.Core
{
    public class IPBlockPolicyFactory
    {
        public IPBlockPolicy GetPolicy(BlockedEntry blockEntry, string source)
        {
            return new IPBlockPolicy(true, DateTime.Now.AddMinutes(1), $"(IP-BLOCK) {source} Blocked IP");
        }
    }
}