namespace Tazqon.Storage.Querys
{
    class MessengerGroupsQuery : Query
    {
        public MessengerGroupsQuery(int CharacterId)
        {
            base.Listen("SELECT * FROM character_messenger_groups WHERE character_id = @id", QueryType.DataTable);
            base.Push("id", CharacterId);
        }
    }

    class MessengerGroupMembersQuery : Query
    {
        public MessengerGroupMembersQuery(int GroupId)
        {
            base.Listen("SELECT character_id FROM character_messenger_group_members WHERE group_id = @id", QueryType.DataTable);
            base.Push("id", GroupId);
        }
    }

    class MessengerFriendsQuery : Query
    {
        public MessengerFriendsQuery(int CharacterId)
        {
            base.Listen("SELECT character_id,friend_id FROM character_friends WHERE character_id = @id AND pending = '0' OR friend_id = @id AND pending = '0'", QueryType.DataTable);
            base.Push("id", CharacterId);
        }
    }

    class MessengerRequestsQuery : Query
    {
        public MessengerRequestsQuery(int CharacterId)
        {
            base.Listen("SELECT character_id FROM character_friends WHERE friend_id = @id AND pending = '1'", QueryType.DataTable);
            base.Push("id", CharacterId);
        }
    }

    class MessengerFriendRemoveQuery : Query
    {
        public MessengerFriendRemoveQuery(int CharacterId, int FriendId)
        {
            base.Listen("DELETE FROM character_friends WHERE character_id = @a AND friend_id = @b OR character_id = @b AND friend_id = @a LIMIT 1", QueryType.Action);
            base.Push("a", CharacterId);
            base.Push("b", FriendId);
        }
    }

    class MessengerCharacterSearchQuery : Query
    {
        public MessengerCharacterSearchQuery(string Username)
        {
            base.Listen("SELECT id FROM characters WHERE username LIKE @username ORDER by username ASC", QueryType.DataTable);
            base.Push("username", "%" + Username + "%");
        }
    }

    class MessengerAcceptFriendQuery : Query
    {
        public MessengerAcceptFriendQuery(int CharacterId, int FriendId)
        {
            base.Listen("UPDATE character_friends SET pending = '0' WHERE character_id = @a AND friend_id = @b LIMIT 1", QueryType.Action);
            base.Push("a", CharacterId);
            base.Push("b", FriendId);
        }
    }

    class MessengerDeclineFriendQuery : Query
    {
        public MessengerDeclineFriendQuery(int CharacterId, int FriendId)
        {
            base.Listen("DELETE FROM character_friends WHERE character_id = @a AND friend_id = @b LIMIT 1", QueryType.Action);
            base.Push("a", CharacterId);
            base.Push("b", FriendId);
        }
    }

    class MessengerDeclineAllFriendQuery : Query
    {
        public MessengerDeclineAllFriendQuery(int CharacterId)
        {
            base.Listen("DELETE FROM character_friends WHERE friend_id = @a", QueryType.Action);
            base.Push("a", CharacterId);
        }
    }

    class MessengerRequestFriendQuery : Query
    {
        public MessengerRequestFriendQuery(int CharacterId, int FriendId)
        {
            base.Listen("INSERT INTO character_friends (character_id, friend_id) VALUES (@a, @b)",QueryType.Action);
            base.Push("a", CharacterId);
            base.Push("b", FriendId);
        }
    }
}
