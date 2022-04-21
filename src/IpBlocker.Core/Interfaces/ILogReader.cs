using Trekco.IpBlocker.Core.Objects;

namespace Trekco.IpBlocker.Core.Interfaces
{
    public interface ILogReader
    {
        List<IpEntry> GetBadIps(DateTime fromDate);
        string GetName();
    }
}