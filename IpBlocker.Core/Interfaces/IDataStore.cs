using IpBlocker.Core.Objects;

namespace IpBlocker.Core.Interfaces
{
    public interface IDataStore
    {
        bool IsIPBlocked(string ipEntryIp, string source, int port, string protocol);
        void Add(BlockedEntry blockEntry, IPBlockPolicy policy, string source);
        void Initialize();
    }
}