namespace Trekco.IpBlocker.Core.Objects
{
    public class IpEntry
    {
        public string IP { get; set; }
        public DateTime Time { get; set; }
        public string Message { get; set; }
        public string ValidationData { get; set; }
        public int[] Ports { get; set; }
        public NetworkProtocol Protocol { get; set; }
    }
}