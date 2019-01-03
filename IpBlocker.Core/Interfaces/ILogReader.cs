using System;
using System.Collections.Generic;

using IpBlocker.Core.Objects;

namespace IpBlocker.Core.Interfaces
{
    public interface ILogReader
    {
        List<IpEntry> GetBadIps(DateTime fromDate);
        string GetName();
    }
}