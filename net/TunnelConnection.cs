using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using usbip_tunnel.net;
using static usbip_tunnel.Program;

namespace usbip_tunnel
{
    public class TunnelConnection : BaseConnection
    {
        public const int MAX_CMD_SESSION = 5;

        public struct TunnelCommand
        {
            public string Name;
            public IList<byte[]> Responses;
        }

        public enum TunnelOperation : long
        {
            [Description("LIST")]
            LIST = 0x0111800500000000,

            [Description("IMPORT")]
            IMPORT = 0x0111800300000000,
        }

        public System.Threading.Timer Timer;

        public AutoResetEvent Locker = new AutoResetEvent(true);

        public IList<ClientConnection> Subscribers { get; }
        public ClientConnection Receiver { get; private set; }
        public IList<TunnelCommand> Commands { get; }

        public ushort Version { get; }

        public TunnelOperation Operation { get; }

        public string BusNum { get; private set; }


        public int SessionSubmitCounter = 0;

        public TunnelConnection(byte[] uid) : base(uid)
        {
            string data = BitConverter.ToString(uid).Replace("-", "");
            this.Operation = (TunnelOperation)long.Parse(data.Substring(0, 16), System.Globalization.NumberStyles.HexNumber);
            //this.BusNum = this.Operation == TunnelOperation.IMPORT ? Encoding.Default.GetString(uid, 8, Array.IndexOf<byte>(uid, 0, 8) - 8) : "";
            this.Subscribers = new List<ClientConnection>();
            this.Commands = new List<TunnelCommand>();


            this.Timer = new System.Threading.Timer((object state) =>
            {
                Log.Info("СРАБОТАЛ ТАЙМЕР ОЖИДАНИЯ");

                //SessionSubmitCounter = 0;

                this.Receiver = null;
                this.Locker.Set();

                //foreach (TunnelCommand c in this.Commands)
                //{
                //    Log.InfoFormat("COMMAND NAME:{0}", c.Name);
                //    foreach (byte[] response in c.Responses)
                //    {
                //        Log.InfoFormat("REP:{0}", BitConverter.ToString(response).Replace("-", ""));
                //    }
                //}
            });
        }

        public override void ReceiveCallback(IAsyncResult result)
        {
            if (result.IsCompleted && this.Socket.Connected)
            {


                SocketError errorCode;
                byte[] bytes = new byte[this.Socket.EndReceive(result, out errorCode)];
                if (errorCode == SocketError.Success)
                {
                    Array.Copy((byte[])result.AsyncState, bytes, bytes.Length);
                    string data = BitConverter.ToString(bytes).Replace("-", "");

                    Log.InfoFormat("Получены данные ({0}) от ТУНЕЛЯ ({1}): {2} ", bytes.Length, this.ToString(), data);

                    ClientConnection receiver = this.Receiver;

                    if (this.Operation == TunnelOperation.IMPORT && this.Status != ConnectionStatus.IMPORTED)
                    {
                        //this.Submited && 

                        this.Commands?[this.Commands.Count - 1].Responses.Add(bytes);
                        this.Submited = (this.Commands[0].Name == "0111800300000000") && (this.Commands[0].Responses.Count > 0) && (BitConverter.ToString(this.Commands[0].Responses[0]).Replace("-", "").Substring(0, 16) == "0111000300000000") && (this.Commands[this.Commands.Count - 1].Name == "00000001FFFFFF07") && (this.Commands[this.Commands.Count - 1].Responses.Count > 0) && (BitConverter.ToString(this.Commands[this.Commands.Count - 1].Responses[0]).Replace("-", "").Substring(0, 16) == "00000003FFFFFF07");
                        this.Canceled = (this.Commands[0].Name == "0111800300000000") && (this.Commands[0].Responses.Count > 0) && (BitConverter.ToString(this.Commands[0].Responses[0]).Replace("-", "").Substring(0, 16) != "0111000300000000") && (this.Commands[0].Responses[this.Commands[0].Responses.Count - 1].Length == 0);

                        if ((this.Commands[0].Name == "0111800300000000") && (this.Commands[0].Responses.Count > 0) && (BitConverter.ToString(this.Commands[0].Responses[0]).Replace("-", "").Substring(0, 16) == "0111000300000000"))
                        {

                        }


                        if (this.Submited && !this.Canceled)
                        {
                            receiver.Submited = this.Submited;
                            receiver.Status = ConnectionStatus.IMPORTED;
                            this.Status = ConnectionStatus.IMPORTED;


                            Log.InfoFormat("ОПЕРАЦИЯ ({0}) ПОДТВЕРЖДЕНА", this.Operation.ToString("G"));

                            TunnelServer.Instance.OnClientJoined(new ClientEventArgs(receiver));

                            this.Receiver = null;
                            this.Locker.Set();
                        }
                        else if (this.Canceled)
                        {
                            Log.InfoFormat("ОШИБКА ИМПОРТА ({0})", this.ToString());

                            this.Receiver.Canceled = true;
                            this.Canceled = true;
                        }
                    }
                    else if (this.Operation == TunnelOperation.LIST)
                    {
                        this.Commands?[this.Commands.Count - 1].Responses.Add(bytes);
                        this.Submited = (this.Commands[0].Name == "0111800500000000") && (this.Commands[0].Responses.Count > 0) && (BitConverter.ToString(this.Commands[0].Responses[0]).Replace("-", "").Substring(0, 16) == "0111000500000000");
                        this.Canceled = this.Submited && (this.Commands[0].Responses[this.Commands[0].Responses.Count - 1].Length == 0);
                    }
                    else if (this.Operation == TunnelOperation.IMPORT && this.Submited && data.Substring(0, 8) == "00000003")
                    {
                        SessionSubmitCounter++;

                        Log.InfoFormat("USBIP_RET ({0}) SEQNUM:{1}; COUNTER:{2}", this.ToString(), data.Substring(8, 8), SessionSubmitCounter);

                        if (SessionSubmitCounter >= MAX_CMD_SESSION)
                        {
                            Log.InfoFormat("+++ ВЫПОЛНЕНО ДОПУСТИМОЕ КОЛИЧЕСТВО КОММАНД ДЛЯ ТУНЕЛЯ МОЖНО СНЯТь БЛОКИРОВКУ");

                            SessionSubmitCounter = 0;

                            this.Receiver = null;
                            this.Locker.Set();
                        }
                    }

                    receiver?.Send(bytes);


                    if (this.Canceled)
                    {
                        Log.InfoFormat("ОПЕРАЦИЯ ({0}) ОТМЕНЯЕТСЯ", this.Operation.ToString("G"));

                        foreach (ClientConnection client in this.Subscribers) client.Close();

                        this.Subscribers.Clear();

                        TunnelServer.Instance.Tunnels.Remove(BitConverter.ToString(this.UID).Replace("-", ""));
                    }
                    else
                    {
                        this.Socket.BeginReceive(this.buffer, 0, this.buffer.Length, SocketFlags.None, ReceiveCallback, this.buffer);
                    }
                }
                else
                {
                    Log.InfoFormat("ОШИБКА ТУНЕЛЯ ({0}) ERROR:", this.ToString(), errorCode.ToString("G"));

                    foreach (ClientConnection client in this.Subscribers) client.Close();

                    this.Subscribers.Clear();

                    TunnelServer.Instance.Tunnels.Remove(BitConverter.ToString(this.UID).Replace("-", ""));
                }
            }
        }

        public override bool Send(byte[] buffer)
        {
            string data = BitConverter.ToString(buffer).Replace("-", "");

            Log.InfoFormat("Переданы данные в ТУНЕЛЬ ({0}): {1}", this.ToString(), data);

            return this.Socket.Send(buffer) > 0;
        }

        public bool Send(byte[] buffer, ClientConnection receiver)
        {
            //Log.InfoFormat("--- ЗАПРОС НА БЛОКИРОВКУ ТУНЕЛЯ ({0}) THREADID:{1}", receiver.Socket.RemoteEndPoint.ToString(), Thread.CurrentThread.ManagedThreadId);

            bool blocked = false;

            if (this.Receiver != receiver)
            {
                Log.InfoFormat("--- ОЖИДАЕМ ОЧЕРЕДЬ В ТУНЕЛЕ (BUSID:{0} СТАТУС:{1}) CLIENT:{2}; THREADID:{3}", this.ToString(), this.Status.ToString("G"), receiver.Socket.RemoteEndPoint.ToString(), Thread.CurrentThread.ManagedThreadId);

                blocked = this.Locker.WaitOne();
                if (blocked)
                {
                    this.Receiver = receiver;
                }
                else
                {
                    return false;
                }

                Log.InfoFormat("--- ЗАБЛОКИРОВАЛИ ТУНЕЛЬ (BUSID:{0} СТАТУС:{1}) CLIENT:{2}; THREADID:{3}", this.ToString(), this.Status.ToString("G"), receiver.Socket.RemoteEndPoint.ToString(), Thread.CurrentThread.ManagedThreadId);
            }


            this.Timer.Change(1000, -1);

            string operation = BitConverter.ToString(buffer).Replace("-", "").Substring(0, 16);

            if (this.Status == ConnectionStatus.IMPORTED && receiver.Status == ConnectionStatus.IMPORT)
            {
                // Если соединение с тунелем уже установлено и данные инициализации получены 
                // а клиент еще нет то выбираем данные для отправки клиенту из архива 

                Log.InfoFormat("ТУНЕЛЬ ({0}) УЖЕ ПОДКЛЮЧЕН", this.ToString());

                TunnelCommand command = this.Commands.First(i => i.Name == operation);
                foreach (byte[] response in command.Responses) this.Receiver?.Send(response);



                return true;
            }
            else if (this.Status != ConnectionStatus.IMPORTED)
            {
                if (operation == "0111800300000000" || operation == "0111800500000000" || operation == "00000001FFFFFF07")
                {
                    this.Commands.Add(new TunnelCommand() { Name = operation, Responses = new List<byte[]>() });
                }

                return this.Send(buffer);
            }
            else
            {
                return this.Send(buffer);
            }
        } 

        public void Lock(ClientConnection receiver)
        {
            Log.InfoFormat("--- ЗАПРОС НА БЛОКИРОВКУ ТУНЕЛЯ ({0}) THREADID:{1}", this.ToString(), Thread.CurrentThread.ManagedThreadId);


            //if (this.Receiver != null)
            //{
            //    while (this.Receiver != receiver && Monitor.IsEntered(this.Receiver))
            //    {
            //        Log.InfoFormat("+++ Ждем освобождения тунеля ClientIP:{0}", this.Receiver.Socket.RemoteEndPoint.ToString());
            //        Monitor.Wait(this.Receiver);
            //        Log.InfoFormat("+++ Тунель свободен UID:{0}", this.ToString());
            //    }



            //    this.Receiver = receiver;
            //    Monitor.Enter(this.Receiver);
            //}
            //else
            //{
            //    this.Receiver = receiver;
            //    Monitor.Enter(this.Receiver);

            //    if (Monitor.IsEntered(this.Receiver))
            //    {
            //        Log.InfoFormat("+++ ТУНЕЛЬ ({0}) ЗАБЛОКИРОВАН КЛИЕНТОМ ({1})", this.ToString(), this.Receiver.Socket.RemoteEndPoint.ToString());
            //    }
            //    else
            //    {
            //        Log.InfoFormat("+++ ТУНЕЛЬ ({0}) НЕУДАЛОСЬ ЗАБЛОКИРОВАТЬ ({1})", this.ToString(), this.Receiver.Socket.RemoteEndPoint.ToString());
            //    }
            //}


            //if (this.Receiver != null && this.Receiver != receiver)
            //{

            //}

            //if (!Monitor.IsEntered(this.Receiver))
            //{
            //}
            //else
            //{
            //    Log.InfoFormat("+++ ТУННЕЛЬ УЖЕ ЗАБЛОКИРОВАН ({0})", this.ToString());
            //}
        }

        public void Unlock()
        {
            Log.InfoFormat("--- ЗАПРОС НА РАЗБЛОКИРОВКУ ТУНЕЛЯ ({0}) THREADID:{1}", this.ToString(), Thread.CurrentThread.ManagedThreadId);

            this.Receiver = null;
            if (this.Locker.Set())
            {
                Log.InfoFormat("+++ БЛОКИРОВКА С ТУНЕЛЯ ({0}) СНЯТА", this.ToString());
            }
            else
            {
                Log.InfoFormat("+++ ТУННЕЛЬ НЕБЫЛ ЗАБЛОКИРОВАН ({0})", this.ToString());
            }

            //if (Monitor.IsEntered(this.Receiver))
            //{
            //    this.Receiver = null;
            //    Monitor.Exit(this.Receiver);

            //    SessionSubmitCounter = 0;

            //    Log.InfoFormat("+++ БЛОКИРОВКА С ТУНЕЛЯ ({0}) СНЯТА", this.ToString());
            //}
            //else
            //{
            //    Log.InfoFormat("+++ ТУННЕЛЬ НЕБЫЛ ЗАБЛОКИРОВАН ({0})", this.ToString());
            //}
        }

        public override string ToString()
        {
            return this.Operation.ToString("G") + (!String.IsNullOrEmpty(this.BusNum) ? ":" + this.BusNum :  ":");
        } 
    }
}
