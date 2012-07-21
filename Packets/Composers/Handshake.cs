using Tazqon.Packets.Headers;
using Tazqon.Packets.Messages;

namespace Tazqon.Packets.Composers
{
    class PolicyFileRequestComposer : PacketComposer
    {
        public PolicyFileRequestComposer(string PolicyFileResponse)
        {
            base.WriteString(PolicyFileResponse);
        }
    }

    class SessionParamsMessageComposer : PacketComposer
    {
        public SessionParamsMessageComposer()
        {
            base.WriteHeader(MessageComposerIds.SessionParamsMessageComposer);
            base.Write(default(int));
        }
    }
    

    class AuthenticationOKMessageComposer : PacketComposer
    {
        public AuthenticationOKMessageComposer()
        {
            base.WriteHeader(MessageComposerIds.AuthenticationOKMessageComposer);
        }
    }
}
