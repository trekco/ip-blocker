using System;

namespace IpBlocker.Core.Objects
{
    public class BlockedEntry
    {
        public BlockedEntry(IpEntry ipEntry)
        {            
            Ip = ipEntry.IP;
            IpLocation = "Unknown";
            Port = ipEntry.Port;
            Protocol = ipEntry.Protocol;
        }

        public bool IsBLocked { get; set; }
        public string RuleName { get; set; }
        public string Ip { get; set; }
        public string IpLocation { get; set; }
        public int Port { get; set; }
        public NetworkProtocol Protocol { get; set; }
    }
}