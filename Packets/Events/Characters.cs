using Tazqon.Network;
using Tazqon.Packets.Composers;
using Tazqon.Packets.Interfaces;

namespace Tazqon.Packets.Events
{
    class InfoRetrieveMessageEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            Session.WriteComposer(new HabboActivityPointNotificationMessageComposer(Session.Character.ActivityPoints));

            Session.Character.UpdateAchievements(System.HabboSystem.AchievementManager.GetCharacterAchievement(Session.Character.Id));
            Session.WriteComposer(new AchievementsScoreComposer(System.HabboSystem.AchievementManager.GetAchievementScore(Session.Character)));
            Session.WriteComposer(new NavigatorSettingsComposer(Session.Character.HomeRoom));
            Session.WriteComposer(new UserRightsMessageComposer(Habbo.Characters.Membership.Vip, Session.Character.Id));

            Session.Character.UpdateLogs(System.HabboSystem.CharacterManager.GetCharacterLogs(Session.Character.Id));

            Session.Character.CheckAchievement(2);
            Session.Character.CheckAchievement(3); // RegistrationDuration
            Session.Character.CheckAchievement(10); // HappyHour
            // TODO serialize ach at login etc
        }
    }

    class GetCreditsInfoEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            Session.WriteComposer(new CreditBalanceComposer(Session.Character.Credits));
        }
    }

    class ScrGetUserInfoMessageEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            Session.WriteComposer(new UserObjectComposer(Session.Character));
        }
    }

    class GetSoundSettingsEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            Session.WriteComposer(new SoundSettingsComposer(Session.Character.SoundSettings));
        }
    }

    class GetIgnoredUsersMessageEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            Session.Character.UpdateIgnores(System.HabboSystem.CharacterManager.GetIgnores(Session.Character.Id));
            Session.WriteComposer(new IgnoredUsersMessageComposer(Session.Character.Ignores));
        }
    }

    class SetSoundSettingsEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            Session.Character.UpdateSoundSettings(Packet.PopInt32());
        }
    }
}