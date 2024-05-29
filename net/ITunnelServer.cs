using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace usbip_tunnel.net
{
    public interface ITunnelServer
    {
        IDictionary<string, TunnelConnection> Tunnels { get; }

    }
}
