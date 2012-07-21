using System.Collections.Generic;
using System.Linq;
using Tazqon.Packets.Messages;
using Tazqon.Habbo.Achievements;
using Tazqon.Habbo.Characters;
using Tazqon.Packets.Headers;

namespace Tazqon.Packets.Composers
{
    class AchievementsScoreComposer : PacketComposer
    {
        public AchievementsScoreComposer(int AchievementScore)
        {
            base.WriteHeader(MessageComposerIds.AchievementsScoreComposer);
            base.Write(AchievementScore);
        }
    }

    class AchievementsComposer : PacketComposer
    {
        public AchievementsComposer(Character Character, IEnumerable<AchievementCategory> Categorys)
        {
            base.WriteHeader(MessageComposerIds.AchievementsComposer);
            base.Write(System.HabboSystem.AchievementManager.GetAchievementAmount());

            foreach (AchievementCategory Category in Categorys)
            {
                foreach (Achievement Achievement in Category.Achievements.Values)
                {
                    int CurrentLevel = Character.GetAchievementProgress(Achievement.Id);

                    int NextLevel = (CurrentLevel + 1);

                    if (NextLevel > Achievement.Levels)
                    {
                        NextLevel = Achievement.Levels;
                    }

                    base.Write(Achievement.Id);
                    base.Write(NextLevel);
                    base.Write(Achievement.GetBadgeCode(NextLevel));
                    base.Write(Achievement.GetRequired(NextLevel));
                    base.Write(Achievement.GetPixelReward(NextLevel));
                    base.Write((int)AchievementRewardType.Pixels);
                    base.Write(Character.GetAchievementProgessLimit(Achievement.Id));
                    base.Write(CurrentLevel == Achievement.Levels);
                    base.Write(Category.Caption.ToLower());
                    base.Write(Achievement.Levels);
                }
            }
        }
    }

    class BadgePointLimitsComposer : PacketComposer
    {
        public BadgePointLimitsComposer(IEnumerable<AchievementCategory> Categorys)
        {
            base.WriteHeader(MessageComposerIds.BadgePointLimitsComposer);
            base.Write(System.HabboSystem.AchievementManager.GetAchievementAmount());

            foreach (AchievementCategory Category in Categorys)
            {
                foreach (Achievement Achievement in Category.Achievements.Values)
                {
                    base.Write(System.HabboSystem.BadgeManager.GetBadgeCode(Achievement.BadgeId).Replace("ACH_", string.Empty));
                    base.Write(Achievement.Limits.Count);

                    foreach (var kvp in (from item in Achievement.Limits orderby item.Key ascending select item))
                    {
                        base.Write(kvp.Key);
                        base.Write(kvp.Value);
                    }
                }
            }
        }
    }

    class AchievementComposer : PacketComposer
    {
        public AchievementComposer(Character Character, Achievement Achievement)
        {
            base.WriteHeader(MessageComposerIds.AchievementComposer);

            int CurrentLevel = Character.GetAchievementProgress(Achievement.Id);

            int NextLevel = (CurrentLevel + 1);

            if (NextLevel > Achievement.Levels)
            {
                NextLevel = Achievement.Levels;
            }

            base.Write(Achievement.Id);
            base.Write(NextLevel);
            base.Write(Achievement.GetBadgeCode(NextLevel));
            base.Write(Achievement.GetRequired(NextLevel));
            base.Write(Achievement.GetPixelReward(NextLevel));
            base.Write((int)AchievementRewardType.Pixels);
            base.Write(Character.GetAchievementProgessLimit(Achievement.Id));
            base.Write(CurrentLevel == Achievement.Levels);
            base.Write(System.HabboSystem.AchievementManager.GetCategory(Achievement.CategoryId).Caption.ToLower());
            base.Write(Achievement.Levels);
        }
    }

    class HabboAchievementNotificationMessageComposer : PacketComposer
    {
        public HabboAchievementNotificationMessageComposer(int Level, Achievement Achievement)
        {
            base.WriteHeader(MessageComposerIds.HabboAchievementNotificationMessageComposer);
            base.Write(Achievement.Id);
            base.Write(Level);
            base.Write(Achievement.BadgeId + Level);
            base.Write(Achievement.GetBadgeCode(Level));
            base.Write(Achievement.GetScoreReward(Level));
            base.Write(Achievement.GetPixelReward(Level));
            base.Write(0); // TODO : Need to find out what this means.
            base.Write(0); // TODO : Extra Achievement Score.
            base.Write(Achievement.BadgeId + (Level - 1));
            base.Write(Achievement.GetBadgeCode(Level - 1 <= 0 ? 1 : Level - 1));
            base.Write(System.HabboSystem.AchievementManager.GetCategory(Achievement.CategoryId).Caption.ToLower());
        }
    }
}
