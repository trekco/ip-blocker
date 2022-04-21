using Trekco.IpBlocker.Core.Objects;

namespace Trekco.IpBlocker.Core
{
    public interface IIPBlockPolicyFactory
    {
        IPBlockPolicy GetPolicy(BlockedEntry blockEntry);
    }
}