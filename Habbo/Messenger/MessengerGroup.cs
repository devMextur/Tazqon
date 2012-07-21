using System.Collections.Generic;
using System.Data;
using Tazqon.Commons.Adapters;
using Tazqon.Storage.Querys;

namespace Tazqon.Habbo.Messenger
{
    class MessengerGroup
    {
        /// <summary>
        /// Id of the MessengerGroup.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// OwnerId of the MessengerGroup.
        /// </summary>
        public int CharacterId { get; private set; }

        /// <summary>
        /// Caption of the MessengerGroup
        /// </summary>
        public string Caption { get; private set; }

        /// <summary>
        /// ICollection of members.
        /// </summary>
        public ICollection<int> Members { get; private set; }

        public MessengerGroup(DataRow Row)
        {
            using (RowAdapter Adapter = new RowAdapter(Row))
            {
                this.Id = Adapter.PopInt32("id");
                this.CharacterId = Adapter.PopInt32("character_id");
                this.Caption = Adapter.PopString("caption");
            }

            Members = new List<int>();

            foreach (DataRow Member in System.MySQLManager.GetObject(new MessengerGroupMembersQuery(Id)).GetOutput<DataTable>().Rows)
            {
                using (RowAdapter Adapter = new RowAdapter(Member))
                {
                    int MemberId = Adapter.PopInt32("character_id");

                    if (!Members.Contains(MemberId))
                    {
                        Members.Add(MemberId);
                    }
                }
            }
        }
    }
}
