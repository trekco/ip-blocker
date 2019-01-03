using System;
using System.Collections.Generic;
using System.Text;

namespace IpBlocker.Core.Interfaces
{
    public interface IIPLocator
    {
        string GetIpLocation(string Ip);
        void Initialize();
    }
}
