using System.Collections.Generic;
using System.Data;
using Tazqon.Commons.Adapters;
using Tazqon.Habbo.Characters;
using Tazqon.Storage.Querys;

namespace Tazqon.Habbo.Achievements
{
    class AchievementManager
    {
        /// <summary>
        /// Achievement category s storage.
        /// </summary>
        public Dictionary<int, AchievementCategory> Categorys { get; private set; }

        public AchievementManager()
        {
            Categorys = new Dictionary<int, AchievementCategory>();

            foreach (DataRow Row in System.MySQLManager.GetObject(new AchievementCategorysQuery()).GetOutput<DataTable>().Rows)
            {
                AchievementCategory Category = new AchievementCategory(Row);

                if (!Categorys.ContainsKey(Category.Id))
                {
                    Categorys.Add(Category.Id, Category);
                }
            }
        }

        /// <summary>
        /// Returns the amount of achievements.
        /// </summary>
        /// <returns></returns>
        public int GetAchievementAmount()
        {
            int Output = default(int);

            foreach (AchievementCategory Category in Categorys.Values)
            {
                Output += Category.Achievements.Count;
            }

            return Output;
        }
        
        /// <summary>
        /// Returns an category with the id.
        /// </summary>
        /// <param name="CategoryId"></param>
        /// <returns></returns>
        public AchievementCategory GetCategory(int CategoryId)
        {
            using (DictionaryAdapter<int, AchievementCategory> DA = new DictionaryAdapter<int, AchievementCategory>(Categorys))
            {
                return DA.TryPopValue(CategoryId);
            }
        }

        /// <summary>
        /// Returns an achievement loaded by the category.
        /// </summary>
        /// <param name="AchievementId"></param>
        /// <returns></returns>
        public Achievement GetAchievement(int AchievementId)
        {
            foreach (AchievementCategory Category in Categorys.Values)
            {
                foreach (Achievement Achievement in Category.Achievements.Values)
                {
                    if (Achievement.Id == AchievementId)
                    {
                        return Achievement;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets achievements for an specified character.
        /// </summary>
        /// <param name="CharacterId"></param>
        /// <returns></returns>
        public Dictionary<int, int> GetCharacterAchievement(int CharacterId)
        {
            Dictionary<int, int> Output = new Dictionary<int, int>();

            foreach (DataRow Row in System.MySQLManager.GetObject(new CharacterAchievementsQuery(CharacterId)).GetOutput<DataTable>().Rows)
            {
                using (RowAdapter Adapter = new RowAdapter(Row))
                {
                    int AchievementId = Adapter.PopInt32("achievement_id");
                    int Progress = Adapter.PopInt32("progress");

                    if (!Output.ContainsKey(AchievementId))
                    {
                        Output.Add(AchievementId, Progress);
                    }
                }
            }

            return Output;
        }

        /// <summary>
        /// Calculates the score of an character.
        /// </summary>
        /// <param name="Character"></param>
        /// <returns></returns>
        public int GetAchievementScore(Character Character)
        {
            int Output = default(int);

            foreach (var kvp in Character.Achievements)
            {
                Achievement Achievement = GetAchievement(kvp.Value);

                if (Achievement == null)
                {
                    continue;
                }

                for (int i = 0; i <= kvp.Value; i++)
                {
                    Output += Achievement.GetScoreReward(i);
                }
            }

            return Output;
        }
    }

    internal enum AchievementRewardType
    {
        Pixels = 0
    }
}