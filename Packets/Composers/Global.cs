using Tazqon.Packets.Messages;
using Tazqon.Packets.Headers;

namespace Tazqon.Packets.Composers
{
    class LatencyPingResponseMessageComposer : PacketComposer
    {
        public LatencyPingResponseMessageComposer(int Interval)
        {
            base.WriteHeader(MessageComposerIds.LatencyPingResponseMessageComposer);
            base.Write(Interval);
        }
    }

    class MOTDNotificationComposer : PacketComposer
    {
        public MOTDNotificationComposer(string Message)
        {
            base.WriteHeader(MessageComposerIds.MOTDNotificationComposer);
            base.Write(true);
            base.Write(Message);
        }
    }

    class HabboBroadcastMessageComposer : PacketComposer
    {
        public HabboBroadcastMessageComposer(string Message)
        {
            base.WriteHeader(MessageComposerIds.HabboBroadcastMessageComposer);
            base.Write(Message);
        }
    }

    class ModMessageComposer : PacketComposer
    {
        public ModMessageComposer(string Message, string Link = "")
        {
            base.WriteHeader(MessageComposerIds.ModMessageComposer);
            base.Write(Message);

            if (Link.Length > 0)
            {
                base.Write(Link);
            }
        }
    }

    class UserNotificationMessageComposer : PacketComposer
    {
        public UserNotificationMessageComposer(string Caption, string Content)
        {
            base.WriteHeader(MessageComposerIds.UserNotificationMessageComposer);
            base.Write(Caption);
            base.Write(Content);
        }
    }

    class GenericErrorComposer : PacketComposer
    {
        public GenericErrorComposer(int ErrorCode)
        {
            base.WriteHeader(MessageComposerIds.GenericErrorComposer);
            base.Write(ErrorCode);
        }
    }
}
