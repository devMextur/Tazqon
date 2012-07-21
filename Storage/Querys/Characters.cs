using System;
namespace Tazqon.Storage.Querys
{
    class CharacterRanksQuery : Query
    {
        public CharacterRanksQuery()
        {
            base.Listen("SELECT * FROM character_ranks ORDER BY id ASC", QueryType.DataTable);
        }
    }

    class CharacterIgnoresQuery : Query
    {
        public CharacterIgnoresQuery(int CharacterId)
        {
            base.Listen("SELECT ignore_id FROM character_ignores WHERE character_id = @id", QueryType.DataTable);
            base.Push("id", CharacterId);
        }
    }

    class CharacterIdQuery : Query
    {
        public CharacterIdQuery(string Username)
        {
            base.Listen("SELECT id FROM characters WHERE username = @name LIMIT 1", QueryType.Integer);
            base.Push("name", Username);
        }
    }

    class CharacterUsernameQuery : Query
    {
        public CharacterUsernameQuery(int CharacterId)
        {
            base.Listen("SELECT username FROM characters WHERE id = @id LIMIT 1", QueryType.String);
            base.Push("id", CharacterId);
        }
    }

    class CharacterFigureQuery : Query
    {
        public CharacterFigureQuery(int CharacterId)
        {
            base.Listen("SELECT figure FROM characters WHERE id = @id LIMIT 1", QueryType.String);
            base.Push("id", CharacterId);
        }
    }

    class CharacterMottoQuery : Query
    {
        public CharacterMottoQuery(int CharacterId)
        {
            base.Listen("SELECT motto FROM characters WHERE id = @id LIMIT 1", QueryType.String);
            base.Push("id", CharacterId);
        }
    }

    class CharacterLastSeenQuery : Query
    {
        public CharacterLastSeenQuery(int CharacterId)
        {
            base.Listen("SELECT last_seen FROM characters WHERE id = @id LIMIT 1", QueryType.String);
            base.Push("id", CharacterId);
        }
    }

    class SetCharacterLastSeenQuery : Query
    {
        public SetCharacterLastSeenQuery(int CharacterId)
        {
            base.Listen("UPDATE characters SET last_seen = @dt WHERE id = @id LIMIT 1", QueryType.Action);
            base.Push("dt", System.GenerateUnixTime());
            base.Push("id", CharacterId);
        }
    }

    class CharacterRankQuery : Query
    {
        public CharacterRankQuery(int CharacterId)
        {
            base.Listen("SELECT rank FROM characters WHERE id = @id LIMIT 1", QueryType.Integer);
            base.Push("id", CharacterId);
        }
    }

    class SetCharacterMutedQuery : Query
    {
        public SetCharacterMutedQuery(int CharacterId, bool Muted)
        {
            base.Listen("UPDATE characters SET muted = @item WHERE id = @id LIMIT 1", QueryType.Action);
            base.Push("item", Muted ? 1 : 0);
            base.Push("id", CharacterId);
        }
    }

    class CharacterAllowNewFriendsQuery : Query
    {
        public CharacterAllowNewFriendsQuery(int CharacterId)
        {
            base.Listen("SELECT allow_new_friends FROM characters WHERE id = @id LIMIT 1", QueryType.Boolean);
            base.Push("id", CharacterId);
        }
    }

    class CharacterAchievementsQuery : Query
    {
        public CharacterAchievementsQuery(int CharacterId)
        {
            base.Listen("SELECT achievement_id,progress FROM character_achievements WHERE character_id = @id", QueryType.DataTable);
            base.Push("id", CharacterId);
        }
    }

    class CharacterActivityPointsQuery : Query
    {
        public CharacterActivityPointsQuery(int CharacterId, int ActivityPoints)
        {
            base.Listen("UPDATE characters SET activity_points = @points WHERE id = @id LIMIT 1", QueryType.Action);
            base.Push("points", ActivityPoints);
            base.Push("id", CharacterId);
        }
    }

    class CharacterLogsQuery : Query
    {
        public CharacterLogsQuery(int CharacterId)
        {
            base.Listen("SELECT * FROM character_logs WHERE character_id = @id", QueryType.DataTable);
            base.Push("id", CharacterId);
        }
    }

    class CharacterLogAddQuery : Query
    {
        public CharacterLogAddQuery(int CharacterId)
        {
            base.Listen("INSERT INTO character_logs (character_id,login_stamp) VALUES (@id,@stamp)", QueryType.Action);
            base.Push("id", CharacterId);

            using (Tazqon.Commons.Adapters.DateTimeAdapter Adapter = new Tazqon.Commons.Adapters.DateTimeAdapter(DateTime.Now))
            {
                base.Push("stamp", Adapter.PopUnixTimestamp());
            }
        }
    }

    class CharacterTimeOnlineQuery : Query
    {
        public CharacterTimeOnlineQuery(int CharacterId, int Increment)
        {
            base.Listen("UPDATE characters SET time_online = time_online + @increment WHERE id = @id LIMIT 1", QueryType.Action);
            base.Push("id", CharacterId);
            base.Push("increment", Increment);
        }
    }

    class CharacterVolumeQuery : Query
    {
        public CharacterVolumeQuery(int CharacterId, int Value)
        {
            base.Listen("UPDATE characters SET sound_settings = @value WHERE id = @id LIMIT 1", QueryType.Action);
            base.Push("id", CharacterId);
            base.Push("value", Value);
        }
    }

}
