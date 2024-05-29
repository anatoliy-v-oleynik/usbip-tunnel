using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace usbip_tunnel.net
{
    public class ClientConnection : BaseConnection
    {
        public static AutoResetEvent Locker = new AutoResetEvent(true);

        private byte[] _buffer;

        public TunnelConnection Tunnel { get; private set; }
        public ClientConnection(Socket socket) : base(socket)
        { 
            this._buffer = new byte[0];
        }

        public override void ReceiveCallback(IAsyncResult result)
        {
            Log.InfoFormat("ReceiveCallback; this.Socket.Available = {0}", this.Socket.Available);

            if (result.IsCompleted && this.Socket.Connected)
            {
                SocketError errorCode;
                int bytesReceive = this.Socket.EndReceive(result, out errorCode);

                if (bytesReceive > 0)
                {
                    int endPosition = _buffer.Length;
                    Array.Resize(ref _buffer, _buffer.Length + bytesReceive);
                    Array.Copy((byte[])result.AsyncState, 0, _buffer, endPosition, bytesReceive);

                    string data = BitConverter.ToString(_buffer).Replace("-", "");


                    Log.InfoFormat("Получены данные ({0}) от КЛИЕНТА ({1}): {2} ", bytesReceive, this.ToString(), data);


                    if (((data.Substring(0, 16) == "0111800300000000") && (data.Length >= 40)) || (data.Substring(0, 16) == "0111800500000000"))
                    {
                        if (!TunnelServer.Instance.Tunnels.ContainsKey(data))
                        {
                            this.Tunnel = new TunnelConnection(_buffer);

                            Log.InfoFormat("НОВЫЙ ТУНЕЛЬ: {0}", this.Tunnel.ToString());

                            TunnelServer.Instance.Tunnels.Add(data, this.Tunnel);
                        }
                        else
                        {
                            this.Tunnel = TunnelServer.Instance.Tunnels[data];
                        }

                        this.Status = (data.Substring(0, 16) == "0111800300000000") ? ConnectionStatus.IMPORT : ConnectionStatus.LIST;

                        this.Tunnel.Subscribers.Add(this);
                    }
                    else if (data.Substring(0, 8) == "00000001" && this.Submited)
                    {
                        Log.InfoFormat("USBIP_CMD ({0}) SEQNUM:{1}", this.Socket.RemoteEndPoint.ToString(), data.Substring(8, 8));
                    }

                    if ((data.Substring(0, 16) == "0111800300000000") && (data.Length >= 40))
                    {
                        Log.InfoFormat("ПОЛНЫЕ ДАННЫЕ ИМПОРТА: {0}", data);

                        this.Tunnel.Send(_buffer, this);

                        _buffer = new byte[0];
                    }
                    else if ((data.Substring(0, 16) != "0111800300000000"))
                    {
                        Log.InfoFormat("ДРУГИЕ ДАННЫЕ: {0}", data);

                        this.Tunnel.Send(_buffer, this);

                        _buffer = new byte[0];
                    }


                }
                else
                {
                    Log.InfoFormat("Получены ПУСТЫЕ ДАННЫЕ от КЛИЕНТА ({0}) ",this.Socket.RemoteEndPoint.ToString());

                    if (this.Tunnel.Receiver == this)
                    {
                        this.Tunnel.Unlock();
                    }

                    TunnelServer.Instance.RemoveClient(this);


                    Log.InfoFormat("КЛИЕНТ ({0}) ЗАКРЫТ", this.ToString());

                    this.Tunnel = null;
                    this.Socket.Close();

                    if (this.Canceled)
                    {

                        
                    }

                    return;
                }

                this.Socket.BeginReceive(this.buffer, 0, this.buffer.Length, SocketFlags.None, ReceiveCallback, this.buffer);
            }
        }

        public override bool Send(byte[] buffer)
        {
            string data = BitConverter.ToString(buffer).Replace("-", "");

            Log.InfoFormat("Переданы данные КЛИЕНТУ ({0}): {1}", this.ToString(), data);

            if (this.Status == ConnectionStatus.IMPORT && data.Length >= 16 &&  data.Substring(0, 16) == "00000003FFFFFF07")
            {
                this.Status = ConnectionStatus.IMPORTED;

                TunnelServer.Instance.OnClientJoined(new ClientEventArgs(this));
            }

            SocketError errorCode;
            return this.Socket.Send(buffer, 0, buffer.Length, SocketFlags.None, out errorCode) > 0;
        }

        public void Close()
        {
            this.Canceled = true;

            if (this.Tunnel.Receiver != this)
            {
                TunnelServer.Instance.Clients.Remove(this);
                this.Socket.Close();

                Log.InfoFormat("КЛИЕНТ ({0}) ЗАКРЫТ", this.ToString());
            }
        }

        public override string ToString()
        {
            return "{ EndPoint:" + this.Socket.RemoteEndPoint.ToString() + ", Status:" + this.Status.ToString("G") + " }";
        }
    }
}
