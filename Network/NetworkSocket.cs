using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

using Tazqon.Network.Components;
using System.Threading;
using Tazqon.Commons.Components;

namespace Tazqon.Network
{
    class NetworkSocket : Socket
    {
        /// <summary>
        /// Stack of Offset's.
        /// </summary>
        public SocketBufferStacker SocketBufferStacker { get; private set; }

        /// <summary>
        /// Stack of SocketAsyncEventArgs.
        /// </summary>
        public SocketArgsStacker SocketArgsStacker { get; private set;}

        /// <summary>
        /// Handles the thread-permissions while processing.
        /// </summary>
        public Semaphore ThreadSupressor { get; private set; }

        /// <summary>
        /// An Integer to give our connections an id.
        /// </summary>
        public SafeInteger SessionsInteger { get; private set; }

        /// <summary>
        /// Storage of our sessions.
        /// </summary>
        public Dictionary<int, Session> Sessions { get; private set; }

        /// <summary>
        /// Returns an ICollection with specified ProgessTypes.
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public ICollection<Session> GetSessions(ProgressType Type)
        {
            ICollection<Session> Output = new List<Session>();

            lock (Sessions)
            {
                foreach (Session Session in Sessions.Values)
                {
                    if (Session.SessionProgress == Type)
                    {
                        Output.Add(Session);
                    }
                }
            }

            return Output;
        }

        /// <summary>
        /// Returns an session with this character id.
        /// </summary>
        /// <param name="CharacterId"></param>
        /// <returns></returns>
        public Session GetSession(int CharacterId)
        {
            lock (Sessions)
            {
                foreach (var Session in GetSessions(ProgressType.Authenticated))
                {
                    if (Session.Character.Id == CharacterId)
                    {
                        return Session;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns an session with this character name.
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public Session GetSession(string Username)
        {
            lock (Sessions)
            {
                foreach (var Session in GetSessions(ProgressType.Authenticated))
                {
                    if (Session.Character.Username == Username)
                    {
                        return Session;
                    }
                }
            }

            return null;
        }

        public NetworkSocket(IPEndPoint EndPoint) : base(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
        {
            base.Bind(EndPoint);
            base.Blocking = false;

            int SessionsNetworkBacklog = System.Configuration.PopInt32("Sessions.Network.Backlog");

            base.Listen(SessionsNetworkBacklog);

            System.IOStreamer.AppendLine("NetworkSocket listening on: {0}", LocalEndPoint);
        }

        /// <summary>
        /// Starts the Environment of AsyncSockets.
        /// </summary>
        public void Serialize()
        {
            int SessionsPoolingAmount = System.Configuration.PopInt32("Sessions.Pooling.Amount");
            int SessionsPoolingBuffersize = System.Configuration.PopInt32("Sessions.Pooling.Buffersize");

            SocketBufferStacker = new SocketBufferStacker(SessionsPoolingAmount, SessionsPoolingBuffersize);
            SocketArgsStacker = new SocketArgsStacker(SessionsPoolingAmount);
            ThreadSupressor = new Semaphore(SessionsPoolingAmount, SessionsPoolingAmount);
            SessionsInteger = new SafeInteger(0, false);
            Sessions = new Dictionary<int, Session>();

            System.IOStreamer.AppendLine("There are {0} EventArgs(s) stacked. [{1}b]", SessionsPoolingAmount, SocketBufferStacker.BufferLength);

            CallAsync(null);
        }

        /// <summary>
        /// Waits for an incoming Socket to handle.
        /// </summary>
        /// <param name="EventArgs"></param>
        public void CallAsync(SocketAsyncEventArgs EventArgs)
        {
            if (EventArgs == null)
            {
                EventArgs = new SocketAsyncEventArgs();
                EventArgs.Completed += EventArgs_Completed;
            }
            else
            {
                EventArgs.AcceptSocket = null;
            }

            ThreadSupressor.WaitOne();

            if (!base.AcceptAsync(EventArgs))
            {
                EventArgs_Completed(null, EventArgs);
            }
        }

        /// <summary>
        /// Handles the incoming Socket.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void EventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                SocketAsyncEventArgs EventArgs = SocketArgsStacker.Pop();
                Session Session = EventArgs.UserToken as Session;

                if (Session != null)
                {
                    Session.Update(SessionsInteger.Push(), e.AcceptSocket, EventArgs);

                    lock (Sessions)
                    {
                        Sessions.Add(Session.Id, Session);
                    }

                    System.IOStreamer.Title = "[" + Sessions.Count + "] Session(s)";

                    if (!Session.Socket.ReceiveAsync(EventArgs))
                    {
                        ProcessReceive(EventArgs);
                    }
                }
            }
            catch (Exception a)
            {
                System.IOStreamer.AppendColor(ConsoleColor.Red);
                System.IOStreamer.AppendLine("[EventArgsCompleted] {0}", a.Message);
            }
            finally
            {
                CallAsync(e);
            }
        }

        /// <summary>
        /// Handles the traffic of the EventArgs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                default:
                    CloseSession(e.UserToken as Session);
                    break;
            }
        }

        /// <summary>
        /// Handles incoming bytes.
        /// </summary>
        /// <param name="EventArgs"></param>
        public void ProcessReceive(SocketAsyncEventArgs EventArgs)
        {
            Session Session = EventArgs.UserToken as Session;

            try
            {
                if (EventArgs.BytesTransferred > 0 && EventArgs.SocketError == SocketError.Success)
                {
                    byte[] Received = new byte[EventArgs.BytesTransferred];

                    Buffer.BlockCopy(SocketBufferStacker.Buffer, EventArgs.Offset, Received, 0, EventArgs.BytesTransferred);

                    System.PacketManager.ProcessBytes(Session, ref Received);
                }
                else
                {
                    CloseSession(Session);
                }
            }
            catch (Exception e)
            {
                System.IOStreamer.AppendColor(ConsoleColor.Red);
                System.IOStreamer.AppendLine("[ProcessReceive] {0}",e.Message);
                CloseSession(Session);
            }
            finally
            {
                if (Session != null && Session.Socket != null)
                {
                    Session.Socket.ReceiveAsync(EventArgs);
                }
            }
        }

        /// <summary>
        /// Handles outgoing bytes.
        /// </summary>
        /// <param name="EventArgs"></param>
        public void ProcessSend(SocketAsyncEventArgs EventArgs)
        {
            if (EventArgs.SocketError != SocketError.Success)
            {
                Session Session = (Session)EventArgs.UserToken;
                CloseSession(Session);
            }
        }

        /// <summary>
        /// Closes and handles the disconnecting session.
        /// </summary>
        /// <param name="Session"></param>
        public void CloseSession(Session Session)
        {
            try
            {
                Session.Socket.Shutdown(SocketShutdown.Both);
                Session.Socket.Close();
            }
            catch { }

            Session.Suppress();

            lock (Sessions)
            {
                Sessions.Remove(Session.Id);
            }

            Session.Clear();

            ThreadSupressor.Release();
            SocketArgsStacker.Push(Session.EventArgs);

            System.IOStreamer.Title = "[" + Sessions.Count + "] Session(s)";
        }
    }
}
