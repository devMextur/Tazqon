using System.Collections.Generic;

namespace Tazqon.Network.Components
{
    class SocketBufferStacker : Stack<int>
    {
        /// <summary>
        /// Saves all bytes for memorying the Sockets.
        /// </summary>
        public byte[] Buffer { get; private set; }

        /// <summary>
        /// Returns the length of the buffer.
        /// </summary>
        public int BufferLength
        {
            get
            {
                return Buffer.Length;
            }
        }

        /// <summary>
        /// Gets the length of each buffer
        /// </summary>
        public int Buffersize { get; private set; }

        public SocketBufferStacker(int Capacity, int Buffersize)
        {
            Buffer = new byte[((Capacity) * (Buffersize * 2))];

            for (int i = Capacity; i >= 0; i--)
            {
                base.Push((i * Buffersize));
            }

            this.Buffersize = Buffersize;
        }

        /// <summary>
        /// Tries to get an Offset, then it will be updated.
        /// </summary>
        /// <param name="Offset"></param>
        /// <returns></returns>
        public bool TryPop(out int Offset)
        {
            if (base.Count > 0)
            {
                Offset = base.Pop();
            }
            else
            {
                Offset = -1;
            }

            return Offset != -1;
        }
    }
}
