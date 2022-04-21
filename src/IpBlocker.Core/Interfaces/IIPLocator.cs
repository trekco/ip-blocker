namespace Trekco.IpBlocker.Core.Interfaces
{
    public interface IIPLocator
    {
        string GetIpLocation(string Ip);
        void Initialize();
    }
}