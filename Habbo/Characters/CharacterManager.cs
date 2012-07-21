using System.Collections.Generic;
using System.Data;
using System.Linq;

using Tazqon.Storage.Querys;
using Tazqon.Commons.Adapters;
using System;
using Tazqon.Network;
using Tazqon.Habbo.Rooms;

namespace Tazqon.Habbo.Characters
{
    class CharacterManager
    {
        /// <summary>
        /// Ranks to couple at characters, to fuse them.
        /// </summary>
        public Dictionary<int, CharacterRank> CharacterRanks { get; private set; }

        /// <summary>
        /// Cache for offline users.
        /// </summary>
        public Dictionary<int, Character> WeakSQLCache { get; private set; }

        public CharacterManager()
        {
            CharacterRanks = new Dictionary<int, CharacterRank>();
            WeakSQLCache = new Dictionary<int, Character>();

            foreach (DataRow Row in System.MySQLManager.GetObject(new CharacterRanksQuery()).GetOutput<DataTable>().Rows)
            {
                CharacterRank Rank = new CharacterRank(Row);
                CharacterRanks.Add(Rank.Id, Rank);
            }
        }

        /// <summary>
        /// Returns an character from Id.
        /// </summary>
        /// <param name="CharacterId"></param>
        /// <returns></returns>
        public bool GetCharacter(int CharacterId, out Character Character)
        {
            Character = null;

            if (WeakSQLCache.ContainsKey(CharacterId))
            {
                Character = WeakSQLCache[CharacterId];
            }
            else
            {
                DataRow Obj = System.MySQLManager.GetObject(new CastCharacterQuery(CharacterId)).GetOutput<DataRow>();

                if (Obj != null)
                {
                    Character = new Character(Obj);

                    WeakSQLCache.Add(CharacterId, Character);
                }

                return Character != null;
            }

            return Character != null;
        }

        /// <summary>
        ///  Returns an character from Username.
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public bool GetCharacter(string Username, out Character Character)
        {
            Character = null;

            foreach(Character xCharacter in WeakSQLCache.Values)
            {
                if (xCharacter.Username.ToLower() == Username.ToLower())
                {
                    Character = xCharacter;
                }
            }

            if (Character != null)
            {
                return true;
            }
            else
            {
                DataRow Obj = System.MySQLManager.GetObject(new CastCharacterQuery(Username)).GetOutput<DataRow>();

                if (Obj != null)
                {
                    Character = new Character(Obj);

                    WeakSQLCache.Add(Character.Id, Character);
                }

                return Character != null;
            }
        }

        /// <summary>
        /// Searches for the user in database, if not valid returns null.
        /// </summary>
        /// <param name="AuthenticateTicket"></param>
        /// <returns></returns>
        public Character Authenticate(string AuthenticateTicket)
        {
            DataRow Obj = System.MySQLManager.GetObject(new AuthenticateQuery(AuthenticateTicket)).GetOutput<DataRow>();

            if (Obj != null)
            {
                return new Character(Obj);
            }

            return null;
        }

        /// <summary>
        /// Tries to get the value of the rank
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public CharacterRank GetRank(int Key)
        {
            using (DictionaryAdapter<int, CharacterRank> DA = new DictionaryAdapter<int, CharacterRank>(CharacterRanks))
            {
                return DA.TryPopValue(Key);
            }
        }

        /// <summary>
        /// Returns an collection of ignored characters.
        /// </summary>
        /// <param name="CharacterId"></param>
        /// <returns></returns>
        public ICollection<int> GetIgnores(int CharacterId)
        {
            ICollection<int> Output = new List<int>();

            foreach (DataRow Row in System.MySQLManager.GetObject(new CharacterIgnoresQuery(CharacterId)).GetOutput<DataTable>().Rows)
            {
                using(RowAdapter Adapter = new RowAdapter(Row))
                {
                    int IgnoreId = Adapter.PopInt32("ignore_id");

                    if (Output.Contains(IgnoreId))
                    {
                        Output.Add(IgnoreId);
                    }
                }
            }

            return Output;
        }

        /// <summary>
        /// Returns the status of an character.
        /// </summary>
        /// <param name="CharacterId"></param>
        /// <returns></returns>
        public CharacterStatus GetStatus(int CharacterId)
        {
            return System.NetworkSocket.GetSession(CharacterId) != null ? CharacterStatus.Online : CharacterStatus.Offline;
        }

        /// <summary>
        /// Returns the status of an character.
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public CharacterStatus GetStatus(string Username)
        {
            return System.NetworkSocket.GetSession(Username) != null ? CharacterStatus.Online : CharacterStatus.Offline;
        }

        /// <summary>
        /// Returns the status of an character.
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public CharacterStatus GetStatus(string Username, out Session Session)
        {
            Session = System.NetworkSocket.GetSession(Username);

            return Session != null ? CharacterStatus.Online : CharacterStatus.Offline;
        }

        /// <summary>
        /// Returns the status of an character.
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public CharacterStatus GetStatus(int Id, out Session Session)
        {
            Session = System.NetworkSocket.GetSession(Id);

            return Session != null ? CharacterStatus.Online : CharacterStatus.Offline;
        }

        /// <summary>
        /// Returns the id of an character.
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public int GetId(string Username)
        {
            Session Session;

            if (GetStatus(Username, out Session) == CharacterStatus.Online)
            {
                return Session.Character.Id;
            }

            Character Character;

            if (GetCharacter(Username, out Character))
            {
                return Character.Id;
            }

            return default(int);
        }

        /// <summary>
        /// Returns an usename, cache or sql.
        /// </summary>
        /// <param name="CharacterId"></param>
        /// <returns></returns>
        public string GetUsername(int CharacterId)
        {
            Session Session;

            if (GetStatus(CharacterId, out Session) == CharacterStatus.Online)
            {
                return Session.Character.Username;
            }

            Character Character;

            if (GetCharacter(CharacterId, out Character))
            {
                return Character.Username;
            }

            return "Unknown Character";
        }

        /// <summary>
        /// Returns an figure, cache or sql.
        /// </summary>
        /// <param name="CharacterId"></param>
        /// <returns></returns>
        public string GetFigure(int CharacterId)
        {
            Session Session;

            if (GetStatus(CharacterId, out Session) == CharacterStatus.Online)
            {
                return Session.Character.Figure;
            }

            Character Character;

            if (GetCharacter(CharacterId, out Character))
            {
                return Character.Figure;
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns an motto, cache or sql.
        /// </summary>
        /// <param name="CharacterId"></param>
        /// <returns></returns>
        public string GetMotto(int CharacterId)
        {
            Session Session;

            if (GetStatus(CharacterId, out Session) == CharacterStatus.Online)
            {
                return Session.Character.Motto;
            }

            return System.MySQLManager.GetObject(new CharacterMottoQuery(CharacterId)).GetOutput<string>();
        }

        /// <summary>
        /// Returns an last seen, cache or sql.
        /// </summary>
        /// <param name="CharacterId"></param>
        /// <returns></returns>
        public string GetLastSeen(int CharacterId)
        {
            Session Session;

            if (GetStatus(CharacterId, out Session) == CharacterStatus.Online)
            {
                using (DateTimeAdapter Adapter = new DateTimeAdapter(Session.Character.LastSeen))
                {
                    return Adapter.GetAlternativestamp();
                }
            }

            Character Character;

            if (GetCharacter(CharacterId, out Character))
            {
                using (DateTimeAdapter Adapter = new DateTimeAdapter(Character.LastSeen))
                {
                    return Adapter.GetAlternativestamp();
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns an alternative name of an character.
        /// </summary>
        /// <param name="CharacterId"></param>
        /// <returns></returns>
        public string GetAlternativeName(int CharacterId)
        {
            if (GetStatus(CharacterId) == CharacterStatus.Offline)
            {
                var Rank = System.MySQLManager.GetObject(new CharacterRankQuery(CharacterId)).GetOutput<int>();
                var RankClass = GetRank(Rank);

                return RankClass.ShowCaptionInGame ? RankClass.Caption : string.Empty;
            }
            else
            {
                var RankClass = System.NetworkSocket.GetSession(CharacterId).Character.Rank;

                return RankClass.ShowCaptionInGame ? RankClass.Caption : string.Empty;
            }
        }

        /// <summary>
        /// Returns an boolean of the specified character his requests-state.
        /// </summary>
        /// <param name="CharacterId"></param>
        /// <returns></returns>
        public bool GetAllowNewFriends(int CharacterId)
        {
            Session Session;

            if (GetStatus(CharacterId, out Session) == CharacterStatus.Online)
            {
                return Session.Character.AllowNewFriends;
            }

            return System.MySQLManager.GetObject(new CharacterAllowNewFriendsQuery(CharacterId)).GetOutput<bool>();
        }

        /// <summary>
        /// Returns an collection of characterlogs.
        /// </summary>
        /// <param name="CharacterId"></param>
        /// <returns></returns>
        public ICollection<DateTime> GetCharacterLogs(int CharacterId)
        {
            ICollection<DateTime> Output = new List<DateTime>();

            foreach (DataRow Row in System.MySQLManager.GetObject(new CharacterLogsQuery(CharacterId)).GetOutput<DataTable>().Rows)
            {
                using (RowAdapter Adapter = new RowAdapter(Row))
                {
                    string Stamp = Adapter.PopString("login_stamp");

                    using (DateTimeAdapter dtAdapter = new DateTimeAdapter(Stamp))
                    {
                        if (!Output.Contains(dtAdapter.PopDateTime()))
                        {
                            Output.Add(dtAdapter.PopDateTime());
                        }
                    }
                }
            }

            return Output;
        }

        /// <summary>
        /// Returns an integer with the amount of days in a row.
        /// </summary>
        /// <param name="Logs"></param>
        /// <returns></returns>
        public int GetDaysInARow(ICollection<DateTime> Logs)
        {
            DateTime Now = DateTime.Now;

            ICollection<int> Filter = new List<int>();

            foreach (var Item in Logs)
            {
                int UltraDay = (Item.Year * 365) + Item.DayOfYear;

                if (!Filter.Contains(UltraDay))
                {
                    Filter.Add(UltraDay);
                }
            }

            var Sort = (from item in Filter orderby item descending select item).ToArray();

            int Output = 1;

            if (Sort.Length > 0)
            {
                if (Sort.First() != ((Now.Year * 365) + Now.DayOfYear))
                {
                    return Output;
                }
            }
            else return Output;

            for (int i = 0; i < Sort.Length; i++)
            {
                int UltraDay = Sort[i];

                if (i + 1 >= Sort.Length)
                {
                    return Output;
                }

                int NextDay = Sort[i + 1];

                if ((UltraDay - 1) == NextDay)
                {
                    Output++;
                }
                else
                {
                    return Output;
                }
            }

            return Output;
        }

        /// <summary>
        /// Returns an collection with the tags of the character.
        /// </summary>
        /// <param name="CharacterId"></param>
        /// <returns></returns>
        public ICollection<string> GetCharacterTags(int CharacterId)
        {
            return null; // TODO: TAGS
        }

        /// <summary>
        /// Returns the current room of an character.
        /// </summary>
        /// <param name="CharacterId"></param>
        /// <returns></returns>
        public int GetCurrentRoom(int CharacterId)
        {
            Session Session;

            if (GetStatus(CharacterId, out Session) == CharacterStatus.Online)
            {
                return Session.Character.ConnectedRoom;
            }

            return default(int);
        }

        /// <summary>
        /// Returns the current room of characterId, and auth him.
        /// </summary>
        /// <param name="CharacterId"></param>
        /// <param name="Adapter"></param>
        /// <returns></returns>
        public bool GetCurrentRoom(int CharacterId, out RoomAdapter Adapter)
        {
            Adapter = null;

            Session Session;

            if (GetStatus(CharacterId, out Session) == CharacterStatus.Online)
            {
                if (Session.Character.ConnectedRoom > 0)
                {
                    Adapter = System.HabboSystem.RoomManager.CastAdapter(Session.Character.ConnectedRoom);
                }
            }

            return Adapter != null;
        }
    }

    enum CharacterStatus
    {
         Online, Offline
    }
}
