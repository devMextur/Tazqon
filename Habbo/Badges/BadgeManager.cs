using System.Collections.Generic;
using System.Data;
using Tazqon.Commons.Adapters;
using Tazqon.Storage.Querys;

namespace Tazqon.Habbo.Badges
{
    class BadgeManager
    {
        /// <summary>
        /// BadgeInfomations of the habbo-game.
        /// </summary>
        public Dictionary<int, BadgeInformation> BadgeInformations { get; private set; }

        public BadgeManager()
        {
            BadgeInformations = new Dictionary<int, BadgeInformation>();

            foreach (DataRow Row in System.MySQLManager.GetObject(new BadgeInformationsQuery()).GetOutput<DataTable>().Rows)
            {
                BadgeInformation Information = new BadgeInformation(Row);

                if (!BadgeInformations.ContainsKey(Information.Id))
                {
                    BadgeInformations.Add(Information.Id, Information);
                }
            }
        }

        /// <summary>
        /// Returns the code of an badge, by the id.
        /// </summary>
        /// <param name="BadgeId"></param>
        /// <returns></returns>
        public string GetBadgeCode(int BadgeId)
        {
            using (DictionaryAdapter<int, BadgeInformation> DA = new DictionaryAdapter<int, BadgeInformation>(BadgeInformations))
            {
                return DA.TryPopValue(BadgeId).BadgeCode;
            }
        }

        /// <summary>
        /// Returns the id of an badge, by the code.
        /// </summary>
        /// <param name="BadgeCode"></param>
        /// <returns></returns>
        public int GetBadgeId(string BadgeCode)
        {
            foreach (BadgeInformation Info in BadgeInformations.Values)
            {
                if (Info.BadgeCode == BadgeCode)
                {
                    return Info.Id;
                }
            }

            return default(int);
        }
    }
}
