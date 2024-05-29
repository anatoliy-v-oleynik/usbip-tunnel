using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace usbip_tunnel.net
{
    public abstract class BaseConnection
    {
        protected readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        
        protected byte[] buffer;

        public byte[] UID { get; protected set;  }

        public Socket Socket { get; }
        public byte[] SeqNum { get; private set; }

        public bool Canceled { get; set; }

        public bool Submited { get; set; }

        public ConnectionStatus Status { get; set; }

        public BaseConnection()
        {
            this.Canceled = false;
            this.Submited = false;
        }

        public BaseConnection(byte[] uid) : this()
        {
            this.UID = uid;
            this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Unspecified);
            this.Socket.Bind(new IPEndPoint(IPAddress.Any, 0));
            this.Socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3250));
            this.buffer = new byte[this.Socket.ReceiveBufferSize];
            this.Socket.BeginReceive(this.buffer, 0, this.buffer.Length, SocketFlags.None, ReceiveCallback, this.buffer);
        }


        public BaseConnection(Socket socket) : this()
        {
            Log.InfoFormat("BaseConnection ({0}), CREATE CLIENT...", socket.RemoteEndPoint.ToString());

            this.Socket = socket;
            this.buffer = new byte[this.Socket.ReceiveBufferSize];
            this.Socket.BeginReceive(this.buffer, 0, this.buffer.Length, SocketFlags.None, ReceiveCallback, this.buffer);

            Log.InfoFormat("BaseConnection ({0}), BeginReceive...", socket.RemoteEndPoint.ToString());
        }

        public abstract bool Send(byte[] buffer);

        public abstract void ReceiveCallback(IAsyncResult result);

    }
}
