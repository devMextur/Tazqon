using System.Collections.Generic;
using System.Data;
using Tazqon.Commons.Adapters;
using Tazqon.Storage.Querys;
using Tazqon.Commons.Storage;
using Tazqon.Habbo.Characters;
using Tazqon.Habbo.Messenger;
using Tazqon.Habbo.Achievements;
using Tazqon.Habbo.Badges;
using System.Threading;
using Tazqon.Habbo.Rooms;

namespace Tazqon.Habbo
{
    class HabboSystem
    {
        /// <summary>
        /// Settings readen from SQL.
        /// </summary>
        public MiniConfigration SQLConfigration { get; private set; }

        /// <summary>
        /// Manager of the global Characters
        /// </summary>
        public CharacterManager CharacterManager { get; private set; }

        /// <summary>
        /// Manager of the global Messengers
        /// </summary>
        public MessengerManager MessengerManager { get; private set; }

        /// <summary>
        /// Manager of the global Achievements
        /// </summary>
        public AchievementManager AchievementManager { get; private set; }

        /// <summary>
        /// Manager of the global badges.
        /// </summary>
        public BadgeManager BadgeManager { get; private set; }

        /// <summary>
        /// Manager of the global rooms.
        /// </summary>
        public RoomManager RoomManager { get; private set; }

        /// <summary>
        /// Timer to handle the TimeOnline-Achievements.
        /// </summary>
        public Timer TimeOnlineTimer { get; private set; }

        public HabboSystem()
        {
            SQLConfigration = new MiniConfigration();
            SQLConfigration.Parse(new Dictionary<string, object>());

            foreach (DataRow Row in System.MySQLManager.GetObject(new MySQLSettingsQuery()).GetOutput<DataTable>().Rows)
            {
                using (var RowAdapter = new RowAdapter(Row))
                {
                    SQLConfigration.Settings.Add(RowAdapter.PopString("key"), Row["value"]);
                }
            }

            CharacterManager = new CharacterManager();
            MessengerManager = new MessengerManager();
            AchievementManager = new AchievementManager();
            BadgeManager = new BadgeManager();
            RoomManager = new RoomManager();

            // Every 15 Minutes.
            TimeOnlineTimer = new Timer(CheckTimeOnline, TimeOnlineTimer, 0, 1000 * 60 * 5);
        }

        /// <summary>
        /// Returns an bool if the setting is an mysql-setting
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public bool CheckValueIfSQLSetting(string Value)
        {
            return Value.StartsWith("mysqlsetting(") && Value.EndsWith(")");
        }

        /// <summary>
        /// Tries te get an result of the sql-settings
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <returns></returns>
        public T GetMySQLSetting<T>(string Key)
        {
            string KeyForSQL = Key;

            if (CheckValueIfSQLSetting(Key))
            {
                KeyForSQL = KeyForSQL.Substring(13, Key.Length - 14);
            }

            using (DictionaryAdapter<string, object> DA = new DictionaryAdapter<string, object>(SQLConfigration.Settings))
            {
                return (T)DA.TryPopValue(KeyForSQL);
            }
        }

        public void CheckTimeOnline(object Obj)
        {
            foreach (var Session in System.NetworkSocket.GetSessions(Network.ProgressType.Authenticated))
            {
                Session.Character.UpdateTimeOnline(5);
                Session.Character.ComposeAchievement(5);
                System.MySQLManager.InvokeQuery(new CharacterTimeOnlineQuery(Session.Character.Id, 5));
            }
        }
    }
}
