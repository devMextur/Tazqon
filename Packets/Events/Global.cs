using Tazqon.Packets.Interfaces;
using Tazqon.Network;
using Tazqon.Packets.Composers;

namespace Tazqon.Packets.Events
{
    class LatencyPingRequestMessageEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            Session.WriteComposer(new LatencyPingResponseMessageComposer(Packet.PopInt32()));
        }
    }

    class LatencyPingReportMessageEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            int A = Packet.PopInt32();
            int B = Packet.PopInt32();

            Session.UpdateLatencyPing(((A + B) / 2));
        }
    }

    class EventLogMessageEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            switch (Packet.PopString())
            {
                case "Performance":
                    switch (Packet.PopString())
                    {
                        case "fps":
                            Session.UpdateFramesPerSecond(int.Parse(Packet.PopString()));
                            break;
                    }
                    break;
            }
        }
    }

    class GetMOTDMessageEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            if (System.Configuration.PopBoolean("MOTD.Notifitation.Enabled"))
            {
                Session.WriteComposer(new MOTDNotificationComposer(System.Configuration.PopString("MOTD.Notifitation.Message")));
            }
        }
    }
}
