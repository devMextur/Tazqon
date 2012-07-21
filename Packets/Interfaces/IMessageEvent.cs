using Tazqon.Network;
using Tazqon.Packets.Messages;

namespace Tazqon.Packets.Interfaces
{
    interface IMessageEvent
    {
        /// <summary>
        /// Handles the incoming packet.
        /// </summary>
        /// <param name="Session">Session to use</param>
        /// <param name="Packet">Message builded in packet.</param>
        void Invoke(Session Session, PacketEvent Packet);
    }
}
