using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Tazqon.Packets.Components;

namespace Tazqon.Packets.Messages
{
    class PacketEvent
    {
        /// <summary>
        /// Packet his Header
        /// </summary>
        public short Id { get; private set; }

        /// <summary>
        /// Packet his content
        /// </summary>
        public byte[] Content { get; private set; }

        /// <summary>
        /// To navigate in content.
        /// </summary>
        public int Pointer { get; private set; }

        /// <summary>
        /// Remaining Lenght of the pointers.
        /// </summary>
        public int RemainingLength
        {
            get
            {
                return (Content.Length - Pointer);
            }
        }

        /// <summary>
        /// Tries to get info and returns the length
        /// </summary>
        /// <param name="Bytes"></param>
        /// <param name="ReadStream"> </param>
        /// <param name="Length"></param>
        public bool TryGetInfo(ref byte[] Bytes, BinaryReader ReadStream, out int Length)
        {
            try
            {
                Length = Base64Encoding.DecodeInt32(ReadStream.ReadBytes(3));
                Id = (short)Base64Encoding.DecodeInt32(ReadStream.ReadBytes(2));

                Content = ReadStream.ReadBytes(Length - 2);

                this.Pointer = 0;
            }
            catch
            {
                Length = 0;
            }

            if (Length > 0)
            {
                Length += 3;
            }

            return Length > 0;
        }

        /// <summary>
        /// Returns an integer decoded.
        /// </summary>
        /// <returns></returns>
        public int PopInt32()
        {
            try
            {
                using (var Stream = new BinaryReader(new MemoryStream(Content)))
                {
                    Stream.ReadBytes(Pointer);

                    byte[] WorkBytes = Stream.ReadBytes(RemainingLength < Wire64Encoding.MAX_INTEGER_BYTE_AMOUNT ? RemainingLength : Wire64Encoding.MAX_INTEGER_BYTE_AMOUNT);

                    int Length;
                    int Result = Wire64Encoding.DecodeInt32(WorkBytes, out Length);

                    Pointer += Length;

                    return Result;
                }
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Returns an string decoded.
        /// </summary>
        /// <returns></returns>
        public string PopString()
        {
            try
            {
                using (var Stream = new BinaryReader(new MemoryStream(Content)))
                {
                    Stream.ReadBytes(Pointer);

                    int Length = Base64Encoding.DecodeInt32(Stream.ReadBytes(2));
                    Pointer += 2;
                    Pointer += Length;

                    string Output = Encoding.ASCII.GetString(Stream.ReadBytes(Length));

                    Output = Output.Replace(Convert.ToChar(1), ' ');
                    Output = Output.Replace(Convert.ToChar(2), ' ');
                    Output = Output.Replace(Convert.ToChar(3), ' ');
                    Output = Output.Replace(Convert.ToChar(9), ' ');

                    return Output;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns and boolean decoded.
        /// </summary>
        /// <returns></returns>
        public bool PopBoolean()
        {
            return PopInt32() > 0;
        }

        /// <summary>
        /// Returns an ICollection of popped integers.
        /// </summary>
        /// <returns></returns>
        public ICollection<int> PopCollection()
        {
            ICollection<int> Output = new List<int>();

            int Length = PopInt32();

            for (int i = 0; i < Length; i++)
            {
                int Obj = PopInt32();

                if (!Output.Contains(Obj))
                {
                    Output.Add(Obj);
                }
            }

            return Output;
        }
    }
}
