using System.Data;
using Tazqon.Commons.Adapters;

namespace Tazqon.Habbo.Badges
{
    class BadgeInformation
    {
        /// <summary>
        /// Id of the badgeinfo
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Code of the badgeinfo
        /// </summary>
        public string BadgeCode { get; private set; }

        public BadgeInformation(DataRow Row)
        {
            using (RowAdapter Adapter = new RowAdapter(Row))
            {
                Id = Adapter.PopInt32("id");
                BadgeCode = Adapter.PopString("badge_code");
            }
        }
    }
}
