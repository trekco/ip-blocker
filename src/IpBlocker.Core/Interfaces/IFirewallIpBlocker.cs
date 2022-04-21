using Trekco.IpBlocker.Core.Objects;

namespace Trekco.IpBlocker.Core.Interfaces
{
    public interface IFirewallIpBlocker
    {
        bool Block(BlockedEntry blockEntry, IPBlockPolicy policy, out string ruleName);
        bool Unblock(BlockedEntry entry);
    }
}