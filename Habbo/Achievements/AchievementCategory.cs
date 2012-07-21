using System.Collections.Generic;
using System.Data;
using Tazqon.Commons.Adapters;
using Tazqon.Storage.Querys;

namespace Tazqon.Habbo.Achievements
{
    class AchievementCategory
    {
        /// <summary>
        /// Id of the AchievementCategory
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Caption of the AchievementCategory
        /// </summary>
        public string Caption { get; private set; }

        /// <summary>
        /// Achievements of the AchievementCategory
        /// </summary>
        public Dictionary<int, Achievement> Achievements { get; private set; }

        public AchievementCategory(DataRow Row)
        {
            using (RowAdapter Adapter = new RowAdapter(Row))
            {
                Id = Adapter.PopInt32("id");
                Caption = Adapter.PopString("caption");
            }

            Achievements = new Dictionary<int, Achievement>();

            foreach (DataRow AchievementRow in System.MySQLManager.GetObject(new AchievementsQuery(Id)).GetOutput<DataTable>().Rows)
            {
                Achievement Achievement = new Achievement(AchievementRow);

                if (!Achievements.ContainsKey(Achievement.Id))
                {
                    Achievements.Add(Achievement.Id, Achievement);
                }
            }
        }
    }
}
