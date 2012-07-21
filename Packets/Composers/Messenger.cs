using System.Collections.Generic;
using Tazqon.Packets.Messages;
using Tazqon.Packets.Headers;
using Tazqon.Habbo.Characters;

namespace Tazqon.Packets.Composers
{
    class MessengerInitComposer : PacketComposer
    {
        public MessengerInitComposer(short MAX_FRIENDS, Character Character)
        {
            base.WriteHeader(MessageComposerIds.MessengerInitComposer);
            base.Write(MAX_FRIENDS);
            base.Write(System.HabboSystem.MessengerManager.MAX_FRIENDS_DEFAULT);
            base.Write(System.HabboSystem.MessengerManager.MAX_FRIENDS_CLUB);
            base.Write(System.HabboSystem.MessengerManager.MAX_FRIENDS_VIP);
            base.Write(Character.MessengerGroups.Count);

            foreach (var MessengerGroup in Character.MessengerGroups)
            {
                base.Write(MessengerGroup.Id);
                base.Write(MessengerGroup.Caption);
            }

            base.Write(Character.MessengerFriends.Count);

            foreach (var MessengerFriend in Character.MessengerFriends)
            {
                CharacterStatus Status = System.HabboSystem.CharacterManager.GetStatus(MessengerFriend);

                base.Write(MessengerFriend);
                base.Write(System.HabboSystem.CharacterManager.GetUsername(MessengerFriend));
                base.Write(true);
                base.Write(Status == CharacterStatus.Online);
                base.Write(System.HabboSystem.CharacterManager.GetCurrentRoom(MessengerFriend));
                base.Write(Status == CharacterStatus.Online ? System.HabboSystem.CharacterManager.GetFigure(MessengerFriend) : string.Empty);
                base.Write(System.HabboSystem.MessengerManager.GetGroupId(Character.MessengerGroups, MessengerFriend));
                base.Write(Status == CharacterStatus.Online ? System.HabboSystem.CharacterManager.GetMotto(MessengerFriend) : string.Empty);
                base.Write(Status == CharacterStatus.Offline ? System.HabboSystem.CharacterManager.GetLastSeen(MessengerFriend) : string.Empty);
                base.Write(System.HabboSystem.CharacterManager.GetAlternativeName(MessengerFriend));
                base.Write(string.Empty);
            }

            base.Write(System.HabboSystem.MessengerManager.MAX_FRIENDS_PER_PAGE);
        }
    }

    class BuddyRequestsComposer : PacketComposer
    {
        public BuddyRequestsComposer(ICollection<int> Requests)
        {
            base.WriteHeader(MessageComposerIds.BuddyRequestsComposer);
            base.Write(Requests.Count);
            base.Write(Requests.Count);

            foreach (int RequestId in Requests)
            {
                base.Write(RequestId);
                base.Write(System.HabboSystem.CharacterManager.GetUsername(RequestId));
                base.Write(System.HabboSystem.CharacterManager.GetFigure(RequestId));
            }
        }
    }

    class FriendListUpdateComposer : PacketComposer
    {
        public FriendListUpdateComposer(Character ToCharacter, Dictionary<int, int> Updates)
        {
            base.WriteHeader(MessageComposerIds.FriendListUpdateComposer);
            base.Write(ToCharacter.MessengerGroups.Count);

            foreach (var MessengerGroup in ToCharacter.MessengerGroups)
            {
                base.Write(MessengerGroup.Id);
                base.Write(MessengerGroup.Caption);
            }

            base.Write(Updates.Count);

            foreach (var kvp in Updates)
            {
                CharacterStatus Status = System.HabboSystem.CharacterManager.GetStatus(kvp.Key);

                base.Write(kvp.Value);
                base.Write(kvp.Key);
                base.Write(System.HabboSystem.CharacterManager.GetUsername(kvp.Key));
                base.Write(true);
                base.Write(Status == CharacterStatus.Online);
                base.Write(System.HabboSystem.CharacterManager.GetCurrentRoom(kvp.Key));
                base.Write(Status == CharacterStatus.Online ? System.HabboSystem.CharacterManager.GetFigure(kvp.Key) : string.Empty);
                base.Write(System.HabboSystem.MessengerManager.GetGroupId(ToCharacter.MessengerGroups, kvp.Key));
                base.Write(Status == CharacterStatus.Online ? System.HabboSystem.CharacterManager.GetMotto(kvp.Key) : string.Empty);
                base.Write(Status == CharacterStatus.Offline ? System.HabboSystem.CharacterManager.GetLastSeen(kvp.Key) : string.Empty);
                base.Write(System.HabboSystem.CharacterManager.GetAlternativeName(kvp.Key));
                base.Write(string.Empty);
            }
        }
    }

    class NewConsoleMessageComposer : PacketComposer
    {
        public NewConsoleMessageComposer(int CharacterId, string Message)
        {
            base.WriteHeader(MessageComposerIds.NewConsoleMessageComposer);
            base.Write(CharacterId);
            base.Write(Message);
        }
    }

    class InstantMessageErrorComposer : PacketComposer
    {
        public InstantMessageErrorComposer(int ErrorCode, int TargetId)
        {
            base.WriteHeader(MessageComposerIds.InstantMessageErrorComposer);
            base.Write(ErrorCode);
            base.Write(TargetId);
        }
    }

    class RoomInviteComposer : PacketComposer
    {
        public RoomInviteComposer(int CharacterId, string Message)
        {
            base.WriteHeader(MessageComposerIds.RoomInviteComposer);
            base.Write(CharacterId);
            base.Write(Message);
        }
    }

    class RoomInviteErrorComposer : PacketComposer
    {
        public RoomInviteErrorComposer()
        {
            base.WriteHeader(MessageComposerIds.RoomInviteErrorComposer);
        }
    }

    class HabboSearchResultComposer : PacketComposer
    {
        public HabboSearchResultComposer(Character Character, ICollection<int> Friends, ICollection<int> NoFriends)
        {
            base.WriteHeader(MessageComposerIds.HabboSearchResultComposer);
            base.Write(Friends.Count);

            foreach (int CharacterId in Friends)
            {
                CharacterStatus Status = System.HabboSystem.CharacterManager.GetStatus(CharacterId);

                base.Write(CharacterId);
                base.Write(System.HabboSystem.CharacterManager.GetUsername(CharacterId));
                base.Write(Status == CharacterStatus.Online ? System.HabboSystem.CharacterManager.GetMotto(CharacterId) : string.Empty);
                base.Write(Status == CharacterStatus.Online);
                base.Write(System.HabboSystem.CharacterManager.GetCurrentRoom(CharacterId)); 
                base.Write(string.Empty);
                base.Write(System.HabboSystem.MessengerManager.GetGroupId(Character.MessengerGroups, CharacterId));
                base.Write(Status == CharacterStatus.Online ? System.HabboSystem.CharacterManager.GetFigure(CharacterId) : string.Empty);
                base.Write(Status == CharacterStatus.Offline ? System.HabboSystem.CharacterManager.GetLastSeen(CharacterId) : string.Empty);
                base.Write(string.Empty);
            }

            base.Write(NoFriends.Count);

            foreach (int CharacterId in NoFriends)
            {
                CharacterStatus Status = System.HabboSystem.CharacterManager.GetStatus(CharacterId);

                base.Write(CharacterId);
                base.Write(System.HabboSystem.CharacterManager.GetUsername(CharacterId));
                base.Write(Status == CharacterStatus.Online ? System.HabboSystem.CharacterManager.GetMotto(CharacterId) : string.Empty);
                base.Write(Status == CharacterStatus.Online);
                base.Write(System.HabboSystem.CharacterManager.GetCurrentRoom(CharacterId));
                base.Write(string.Empty);
                base.Write(System.HabboSystem.MessengerManager.GetGroupId(Character.MessengerGroups, CharacterId));
                base.Write(Status == CharacterStatus.Online ? System.HabboSystem.CharacterManager.GetFigure(CharacterId) : string.Empty);
                base.Write(Status == CharacterStatus.Offline ? System.HabboSystem.CharacterManager.GetLastSeen(CharacterId) : string.Empty);
                base.Write(string.Empty);
            }
        }
    }

    class NewBuddyRequestComposer : PacketComposer
    {
        public NewBuddyRequestComposer(int CharacterId, string Username, string Figure)
        {
            base.WriteHeader(MessageComposerIds.NewBuddyRequestComposer);
            base.Write(CharacterId);
            base.Write(Username);
            base.Write(Figure);
        }
    }

    class MessengerErrorComposer : PacketComposer
    {
        public MessengerErrorComposer(int ErrorId, int SampleId)
        {
            base.WriteHeader(MessageComposerIds.MessengerErrorComposer);
            base.Write(ErrorId);
            base.Write(SampleId);
        }
    }
}
