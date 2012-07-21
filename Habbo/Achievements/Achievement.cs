using System.Collections.Generic;
using System.Data;
using Tazqon.Commons.Adapters;
using Tazqon.Storage.Querys;

namespace Tazqon.Habbo.Achievements
{
    class Achievement
    {
        /// <summary>
        /// Id of the Achievement
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// CategoryId of the Achievement
        /// </summary>
        public int CategoryId { get; private set; }

        /// <summary>
        /// BadgeId of the Achievement
        /// </summary>
        public int BadgeId { get; private set; }

        /// <summary>
        /// Pixel reward of the Achievement
        /// </summary>
        public int PixelReward { get; private set; }

        /// <summary>
        /// Score reward of the Achievement
        /// </summary>
        public int ScoreReward { get; private set; }

        /// <summary>
        /// Pixel multiply of the Achievement
        /// </summary>
        public double PixelMultiply { get; private set; }

        /// <summary>
        /// Score multiply of the Achievement
        /// </summary>
        public double ScoreMultiply { get; private set; }

        /// <summary>
        /// Enables when the system would build badges.
        /// </summary>
        public bool EnableBadgeBuilding { get; private set; }

        /// <summary>
        /// Limits of levels.
        /// </summary>
        public Dictionary<int, int> Limits { get; private set; }

        /// <summary>
        /// Level amount of the achievement.
        /// </summary>
        public int Levels
        {
            get
            {
                return Limits.Count > 0 ? Limits.Count : 1;
            }
        }

        public Achievement(DataRow Row)
        {
            using (RowAdapter Adapter = new RowAdapter(Row))
            {
                Id = Adapter.PopInt32("id");
                CategoryId = Adapter.PopInt32("category_id");
                BadgeId = Adapter.PopInt32("badge_id");
                PixelReward = Adapter.PopInt32("pixel_reward");
                ScoreReward = Adapter.PopInt32("score_reward");
                PixelMultiply = Adapter.PopDouble("pixel_multiply");
                ScoreMultiply = Adapter.PopDouble("score_multiply");
                EnableBadgeBuilding = Adapter.PopBoolean("enable_bagde_building");
            }

            Limits = new Dictionary<int, int>();

            foreach (DataRow LimitRow in System.MySQLManager.GetObject(new AchievementLimitsQuery(Id)).GetOutput<DataTable>().Rows)
            {
                using (RowAdapter Adapter = new RowAdapter(LimitRow))
                {
                    int Level = Adapter.PopInt32("level");
                    int Limit = Adapter.PopInt32("required");

                    if (Limit < 0)
                    {
                        Limit = 1;
                    }

                    if (!Limits.ContainsKey(Level))
                    {
                        Limits.Add(Level, Limit);
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the code for the badge.
        /// </summary>
        /// <param name="Level"></param>
        /// <returns></returns>
        public string GetBadgeCode(int Level)
        {
            return EnableBadgeBuilding ? string.Format("{0}{1}", System.HabboSystem.BadgeManager.GetBadgeCode(BadgeId), Level) : System.HabboSystem.BadgeManager.GetBadgeCode(BadgeId);
        }

        /// <summary>
        /// Calculates how much you need for a specified level.
        /// </summary>
        /// <param name="Level"></param>
        /// <returns></returns>
        public int GetRequired(int Level)
        {
            using (DictionaryAdapter<int, int> DA = new DictionaryAdapter<int, int>(Limits))
            {
                return DA.TryPopValue(Level);
            }
        }

        /// <summary>
        /// Calculates how much you get for a specified level
        /// </summary>
        /// <param name="Level"></param>
        /// <returns></returns>
        public int GetPixelReward(int Level)
        {
            return (int)(PixelReward * (Level * PixelMultiply));
        }

        /// <summary>
        /// Calculates how much you get for a specified level
        /// </summary>
        /// <param name="Level"></param>
        /// <returns></returns>
        public int GetScoreReward(int Level)
        {
            return (int)(ScoreReward * (Level * ScoreMultiply));
        }
    }
}