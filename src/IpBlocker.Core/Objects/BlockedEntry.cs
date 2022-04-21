namespace Trekco.IpBlocker.Core.Objects
{
    public class BlockedEntry
    {
        public BlockedEntry()
        {
        }

        public BlockedEntry(IpEntry ipEntry, string source)
        {
            Ip = ipEntry.IP;
            IpLocation = "Unknown";
            Ports = ipEntry.Ports;
            Protocol = ipEntry.Protocol;
            Source = source;
        }

        public bool IsBLocked { get; set; }
        public string RuleName { get; set; }
        public string Ip { get; set; }
        public string IpLocation { get; set; }
        public string Source { get; set; }
        public int[] Ports { get; set; }
        public NetworkProtocol Protocol { get; set; }
    }
}