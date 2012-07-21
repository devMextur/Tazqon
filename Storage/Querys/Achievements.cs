namespace Tazqon.Storage.Querys
{
    class AchievementCategorysQuery : Query
    {
        public AchievementCategorysQuery()
        {
            base.Listen("SELECT * FROM achievement_categorys WHERE enabled = '1'",QueryType.DataTable);
        }
    }

    class AchievementsQuery : Query
    {
        public AchievementsQuery(int CategoryId)
        {
            base.Listen("SELECT * FROM achievements WHERE category_id = @id", QueryType.DataTable);
            base.Push("id", CategoryId);
        }
    }

    class AchievementLimitsQuery : Query
    {
        public AchievementLimitsQuery(int AchievementId)
        {
            base.Listen("SELECT * FROM achievement_limits WHERE achievement_id = @id", QueryType.DataTable);
            base.Push("id", AchievementId);
        }
    }

    class AchievementCharacterProgressQuery : Query
    {
        public AchievementCharacterProgressQuery(int CharacterId, int AchievementId, int Level)
        {
            base.Listen(
                Level > 1
                    ? "UPDATE character_achievements SET progress = @level WHERE character_id = @character_id AND achievement_id = @achievement_id LIMIT 1"
                    : "INSERT INTO character_achievements (character_id,achievement_id) VALUES (@character_id,@achievement_id)",
                QueryType.Action);

            base.Push("character_id", CharacterId);
            base.Push("achievement_id", AchievementId);
            base.Push("level", Level);
        }
    }
}
