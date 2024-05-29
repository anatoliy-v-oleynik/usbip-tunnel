using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace usbip_tunnel.net
{
    public enum ConnectionStatus
    {
        [Description("NONE")]
        NONE,

        [Description("LIST")]
        LIST,

        [Description("IMPORT")]
        IMPORT,

        [Description("IMPORTED")]
        IMPORTED
    }
}
