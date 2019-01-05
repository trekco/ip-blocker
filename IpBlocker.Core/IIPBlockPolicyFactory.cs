using IpBlocker.Core.Objects;

namespace IpBlocker.Core
{
    public interface IIPBlockPolicyFactory
    {
        IPBlockPolicy GetPolicy(BlockedEntry blockEntry);
    }
}