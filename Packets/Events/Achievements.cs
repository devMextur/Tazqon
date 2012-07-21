using Tazqon.Packets.Interfaces;
using Tazqon.Packets.Composers;

namespace Tazqon.Packets.Events
{
    class GetAchievementsEvent : IMessageEvent
    {
        public void Invoke(Network.Session Session, Messages.PacketEvent Packet)
        {
            Session.WriteComposer(new AchievementsComposer(Session.Character, System.HabboSystem.AchievementManager.Categorys.Values));
        }
    }

    class GetBadgePointLimitsEvent : IMessageEvent
    {
        public void Invoke(Network.Session Session, Messages.PacketEvent Packet)
        {
            Session.WriteComposer(new BadgePointLimitsComposer(System.HabboSystem.AchievementManager.Categorys.Values));
        }
    }

    // TODO : Broadcast achievement @ Messenger
}
