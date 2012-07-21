using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Tazqon.Network.Components
{
    class SocketArgsStacker : Stack<SocketAsyncEventArgs>
    {
        public SocketArgsStacker(int Capacity) : base(Capacity)
        {
            for (int i = Capacity; i >= 0; i--)
            {
                SocketAsyncEventArgs EventArgs = new SocketAsyncEventArgs();
                EventArgs.Completed += System.NetworkSocket.IO_Completed;
                Session Session = new Session();
                Session.Activate();
                EventArgs.UserToken = Session;

                int Offset;

                if (System.NetworkSocket.SocketBufferStacker.TryPop(out Offset))
                {
                    EventArgs.SetBuffer(System.NetworkSocket.SocketBufferStacker.Buffer, Offset, System.NetworkSocket.SocketBufferStacker.Buffersize);
                }
                else
                {
                    System.IOStreamer.AppendColor(ConsoleColor.Red);
                    System.IOStreamer.AppendLine("Could not give SocketAsyncEventArgs #{0} his buffer.", i);
                    continue;
                }

                base.Push(EventArgs);
            }
        }
    }
}
