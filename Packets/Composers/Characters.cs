using System.Collections.Generic;
using Tazqon.Packets.Messages;
using Tazqon.Packets.Headers;
using Tazqon.Habbo.Characters;

namespace Tazqon.Packets.Composers
{
    class CreditBalanceComposer : PacketComposer
    {
        public CreditBalanceComposer(int Credits)
        {
            base.WriteHeader(MessageComposerIds.CreditBalanceComposer);
            base.Write(Credits + string.Empty);
        }
    }

    class HabboActivityPointNotificationMessageComposer : PacketComposer
    {
        public  HabboActivityPointNotificationMessageComposer(int ActivityPoints)
        {
            base.WriteHeader(MessageComposerIds.HabboActivityPointNotificationMessageComposer);
            base.Write(ActivityPoints);
        }
    }

    class UserRightsMessageComposer : PacketComposer
    {
        public UserRightsMessageComposer(Membership Membership, int CharacterId)
        {
            base.WriteHeader(MessageComposerIds.UserRightsMessageComposer);
            base.Write((int)Membership);
            base.Write(CharacterId);
        }
    }

    class UserObjectComposer : PacketComposer
    {
        public UserObjectComposer(Character Character)
        {
            base.WriteHeader(MessageComposerIds.UserObjectComposer);
            base.Write(Character.Id + string.Empty);
            base.Write(Character.Username);
            base.Write(Character.Figure);
            base.Write(Character.Gender);
            base.Write(Character.Motto);
            base.Write("String1"); // TODO
            base.Write(1);
            base.Write(Character.RespectEarned); 
            base.Write(Character.RespectLeftHuman);
            base.Write(Character.RespectLeftAnimal); 
        }
    }

    class SoundSettingsComposer : PacketComposer
    {
        public SoundSettingsComposer(int SoundSettings)
        {
            base.WriteHeader(MessageComposerIds.SoundSettingsComposer);
            base.Write(SoundSettings);
        }
    }

    class IgnoredUsersMessageComposer : PacketComposer
    {
        public IgnoredUsersMessageComposer(ICollection<int> Ignores)
        {
            base.WriteHeader(MessageComposerIds.IgnoredUsersMessageComposer);
            base.Write(Ignores.Count);

            foreach (int IgnoreId in Ignores)
            {
                base.Write(IgnoreId);
            }
        }
    }

    class NavigatorSettingsComposer : PacketComposer
    {
        public NavigatorSettingsComposer(int HomeRoom)
        {
            base.WriteHeader(MessageComposerIds.NavigatorSettingsComposer);
            base.Write(HomeRoom);
        }
    }
}
