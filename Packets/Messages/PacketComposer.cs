using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tazqon.Packets.Components;

namespace Tazqon.Packets.Messages
{
    class PacketComposer
    {
        /// <summary>
        /// Storage of bytes to write.
        /// </summary>
        public List<byte> Stream { get; private set; }

        /// <summary>
        /// Enables to write Raw-Packets.
        /// </summary>
        public bool CloseBuffer { get; private set; }

        public PacketComposer()
        {
            Stream = new List<byte>();
            CloseBuffer = false;
        }

        /// <summary>
        /// Writes and header to the stream.
        /// </summary>
        /// <param name="Header"></param>
        public void WriteHeader(short Header)
        {
            if (Header < 0)
            {
                return;
            }

            if (CloseBuffer)
            {
                Stream.Add(1); // Multiply packets composing.
            }

            byte[] Bytes = Base64Encoding.EncodeInt32(Header, 2);

            Stream.AddRange(Bytes);

            CloseBuffer = true;
        }

        /// <summary>
        /// Converts an object to bytes and writes on stream.
        /// </summary>
        /// <param name="e"></param>
        public void Write(object e)
        {
            if (e == null)
            {
                return;
            }

            if (e is int)
            {
                Stream.AddRange(Wire64Encoding.EncodeInt32((int)e));
            }
            else if (e is short)
            {
                Stream.AddRange(Wire64Encoding.EncodeInt32((short)e));
            }
            else if (e is bool)
            {
                Stream.AddRange(Wire64Encoding.EncodeInt32((bool)e ? 1 : 0));
            }
            else if (e is PacketComposer)
            {
                Stream.AddRange((e as PacketComposer).GetOutput());
            }
            else
            {
                Stream.AddRange(Encoding.ASCII.GetBytes(e.ToString()));
                Stream.Add(2);
            }
        }

        /// <summary>
        /// Writes an raw string to the stream.
        /// </summary>
        /// <param name="e"></param>
        public void WriteString(string e)
        {
            Stream.AddRange(Encoding.ASCII.GetBytes(e));
        }

        /// <summary>
        /// Returns an array of bytes resulted by the stream.
        /// </summary>
        /// <returns></returns>
        public byte[] GetOutput()
        {
            if (CloseBuffer)
            {
                if (Stream.Last() != 1)
                {
                    Stream.Add(1);
                }
            }

            return Stream.ToArray();
        }
    }
}
