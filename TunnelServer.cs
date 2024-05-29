using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using usbip_tunnel.net;

namespace usbip_tunnel
{
    public class TunnelServer : ServiceBase, ITunnelServer
    {
        public readonly static TunnelServer Instance = new TunnelServer();


        public ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Socket Socket { get; private set; }

        public IList<ClientConnection> Clients { get; }

        public IDictionary<string, TunnelConnection> Tunnels { get; }


        public event EventHandler<EventArgs> Started;
        public event EventHandler<EventArgs> Stopped;
        public event EventHandler<ClientEventArgs> ClientConnected;
        public event EventHandler<ClientEventArgs> ClientDisconnected;
        public event EventHandler<ClientEventArgs> ClientJoined;

        public TunnelServer()
        {
            this.Clients = new List<ClientConnection>();
            this.Tunnels = new Dictionary<string, TunnelConnection>();
        }

        public void Start(string[] args)
        {
            OnStart(args);
        }

        protected override void OnStart(string[] args)
        {
            if (this.Socket == null)
            {
                this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Unspecified);
                this.Socket.Bind(new IPEndPoint(IPAddress.Any, 3240));
                this.Socket.Listen(5);
                this.Socket.BeginAccept(AcceptCallback, null);

                this.Started(this, new EventArgs());

                Log.Info("SERVER STARTED");
            }
        }

        protected override void OnStop()
        {
            if (this.Socket != null)
            {
                this.Socket.Close();
                this.Socket = null;

                this.Stopped(this, new EventArgs());

                Log.Info("SERVER STOPPED");
            }
        }

        private void AcceptCallback(IAsyncResult result)
        {
            try
            {
                if (this.Socket != null)
                {
                    Socket socket = this.Socket.EndAccept(result);

                    ClientConnection client = new ClientConnection(socket);

                    this.Clients.Add(client);

                    Log.InfoFormat("USBIPClient connected ({0}), waiting for request...", socket.RemoteEndPoint.ToString());

                    ClientConnected(this, new ClientEventArgs(client));

                    this.Socket.BeginAccept(AcceptCallback, null);
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("USBIPClient connected ERROR ({0})", ex.Message);

                return;
            }
        }

        public void RemoveClient(ClientConnection client)
        {
            this.Clients.Remove(client);

            this.ClientDisconnected(this, new ClientEventArgs(client));
        }

        public void OnClientJoined(ClientEventArgs e)
        {
            this.ClientJoined(this, e);
        }
    }
}
