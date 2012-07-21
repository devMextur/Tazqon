using System.Data;
using Tazqon.Commons.Adapters;

namespace Tazqon.Habbo.Characters
{
    class CharacterRank
    {
        /// <summary>
        /// Id of class
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Name of item
        /// </summary>
        public string Caption { get; private set; }

        /// <summary>
        /// Boolean for showing the rank after his name (messenger)
        /// </summary>
        public bool ShowCaptionInGame { get; private set; }

        public CharacterRank(DataRow Row)
        {
            using (RowAdapter Adapter = new RowAdapter(Row))
            {
                this.Id = Adapter.PopInt32("id");
                this.Caption = Adapter.PopString("caption");
                this.ShowCaptionInGame = Adapter.PopBoolean("show_caption_ingame");
            }
        }
    }
}
