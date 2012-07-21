using System;
using Tazqon.Packets.Interfaces;
using Tazqon.Packets.Composers;
using Tazqon.Habbo.Characters;
using Tazqon.Network;

namespace Tazqon.Packets.Events
{
    class InitCryptoMessageEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            Session.UpdateSessionProgress(ProgressType.PolicyRequest);
            Session.WriteComposer(new SessionParamsMessageComposer());
        }
    }

    class SSOTicketMessageEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            string Ticket = Packet.PopString();

            Character CharObj = System.HabboSystem.CharacterManager.Authenticate(Ticket);

            if (CharObj == null)
            {
                System.IOStreamer.AppendColor(ConsoleColor.Red);
                System.IOStreamer.AppendLine("Failed to Authenticate: {0}", Ticket);
                System.NetworkSocket.CloseSession(Session);
            }
            else
            {
                Session.UpdateCharacter(CharObj);
                Session.WriteComposer(new AuthenticationOKMessageComposer());
            }
        }
    }
}
