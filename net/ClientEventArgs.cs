using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace usbip_tunnel.net
{
    public class ClientEventArgs : EventArgs
    {
        public ClientConnection Client { get; }

        public ClientEventArgs(ClientConnection client)
        {
            this.Client = client;
        }

    }
}
