using IpBlocker.Core;
using IpBlocker.Core.Objects;

namespace IpBlocker.Core.Interfaces
{
    public interface IFirewallIpBlocker
    {
        bool Block(BlockedEntry blockEntry, IPBlockPolicy policy, out string ruleName);
        bool Unblock(BlockedEntry entry);
    }
}