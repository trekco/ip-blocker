using System;
using System.Collections.Generic;
using IpBlocker.Core.Objects;

namespace IpBlocker.Core.Interfaces
{
    public interface IDataStore
    {

        string GetConfigValue(string id);
        void SaveConfigValue(string id, string value);
        bool IsIPBlocked(string ipEntryIp, string source, int[] ports, string protocol);
        void Add(BlockedEntry blockEntry, IPBlockPolicy policy, string source);
        void Initialize();
        List<BlockedEntry> GetIpsToUnblock(DateTime before);
        void UnblockBlockedIpRecord(BlockedEntry entry);
    }
}