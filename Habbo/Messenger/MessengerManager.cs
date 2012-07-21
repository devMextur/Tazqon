using System.Collections.Generic;
using System.Data;
using Tazqon.Commons.Adapters;
using Tazqon.Storage.Querys;
using Tazqon.Habbo.Characters;
using Tazqon.Network;
using Tazqon.Packets.Composers;

namespace Tazqon.Habbo.Messenger
{
    class MessengerManager
    {
        /// <summary>
        /// Default value of maximal friends.
        /// </summary>
        public short MAX_FRIENDS_DEFAULT { get; private set; }

        /// <summary>
        /// Club members value of maximal friends.
        /// </summary>
        public short MAX_FRIENDS_CLUB { get; private set; }

        /// <summary>
        /// Vip members value of maximal friends.
        /// </summary>
        public short MAX_FRIENDS_VIP { get; private set; }

        /// <summary>
        /// Maximal friends per page.
        /// </summary>
        public short MAX_FRIENDS_PER_PAGE { get; private set; }

        /// <summary>
        /// Update for the messenger
        /// </summary>
        public ICollection<MessengerUpdate> UpdateRequests { get; private set; }

        public MessengerManager()
        {
            this.MAX_FRIENDS_DEFAULT = (short)System.Configuration.PopInt32("Messenger.Maximal.Friends.Default");
            this.MAX_FRIENDS_CLUB = (short)System.Configuration.PopInt32("Messenger.Maximal.Friends.Club");
            this.MAX_FRIENDS_VIP = (short)System.Configuration.PopInt32("Messenger.Maximal.Friends.Vip");
            this.MAX_FRIENDS_PER_PAGE = (short)System.Configuration.PopInt32("Messenger.Maximal.Friens.Per.Page");

            this.UpdateRequests = new List<MessengerUpdate>();
        }

        /// <summary>
        /// Returns an colletion of messengergroups
        /// </summary>
        /// <param name="CharacterId"></param>
        /// <returns></returns>
        public ICollection<MessengerGroup> GetMessengerGroups(int CharacterId)
        {
            ICollection<MessengerGroup> Output = new List<MessengerGroup>();

            foreach (DataRow Row in System.MySQLManager.GetObject(new MessengerGroupsQuery(CharacterId)).GetOutput<DataTable>().Rows)
            {
                MessengerGroup Group = new MessengerGroup(Row);
                Output.Add(Group);
            }

            return Output;
        }

        /// <summary>
        /// Returns an integer of the friend his group.
        /// </summary>
        /// <param name="Groups"></param>
        /// <param name="FriendId"></param>
        /// <returns></returns>
        public int GetGroupId(ICollection<MessengerGroup> Groups, int FriendId)
        {
            foreach (MessengerGroup Group in Groups)
            {
                foreach (int MemberId in Group.Members)
                {
                    if (MemberId == FriendId)
                    {
                        return Group.Id;
                    }
                }
            }

            return default(int);
        }

        /// <summary>
        /// Returns an collection of messengerfriends
        /// </summary>
        /// <param name="CharacterId"></param>
        /// <returns></returns>
        public ICollection<int> GetMessengerFriends(int CharacterId)
        {
            ICollection<int> Output = new List<int>();

            foreach (DataRow Row in System.MySQLManager.GetObject(new MessengerFriendsQuery(CharacterId)).GetOutput<DataTable>().Rows)
            {
                using (RowAdapter Adapter = new RowAdapter(Row))
                {
                    int xCharacterId = Adapter.PopInt32("character_id");
                    int xFriendId = Adapter.PopInt32("friend_id");

                    if (xCharacterId == CharacterId)
                    {
                        if (!Output.Contains(xFriendId))
                        {
                            Output.Add(xFriendId);
                        }
                    }
                    else
                    {
                        if (!Output.Contains(xCharacterId))
                        {
                            Output.Add(xCharacterId);
                        }
                    }
                }
            }

            return Output;
        }

        /// <summary>
        /// Returns an collection of online messengerfriends
        /// </summary>
        /// <param name="CharacterId"></param>
        /// <returns></returns>
        public ICollection<int> GetOnlineMessengerFriends(int CharacterId)
        {
            ICollection<int> Output = new List<int>();

            foreach (int FriendId in GetMessengerFriends(CharacterId))
            {
                if (System.HabboSystem.CharacterManager.GetStatus(FriendId) == CharacterStatus.Online)
                {
                    Output.Add(FriendId);
                }
            }

            return Output;
        }

        /// <summary>
        /// Returns an collection of messengerrequests
        /// </summary>
        /// <param name="CharacterId"></param>
        /// <returns></returns>
        public ICollection<int> GetMessengerRequests(int CharacterId)
        {
            ICollection<int> Output = new List<int>();

            foreach (DataRow Row in System.MySQLManager.GetObject(new MessengerRequestsQuery(CharacterId)).GetOutput<DataTable>().Rows)
            {
                using (RowAdapter Adapter = new RowAdapter(Row))
                {
                    int xCharacterId = Adapter.PopInt32("character_id");

                    if (!Output.Contains(xCharacterId))
                    {
                        Output.Add(xCharacterId);
                    }
                }
            }

            return Output;
        }

        /// <summary>
        /// Returns an limit of the character.
        /// </summary>
        /// <param name="CharacterId"></param>
        /// <returns></returns>
        public int GetLimitFriends(int CharacterId)
        {
            return MAX_FRIENDS_DEFAULT; // TODO
        }

        /// <summary>
        /// Event for being online / offline
        /// </summary>
        /// <param name="CharacterId"></param>
        public void ParseStatus(int CharacterId)
        {
            foreach (int FriendId in GetOnlineMessengerFriends(CharacterId))
            {
                Session FriendSession = System.NetworkSocket.GetSession(FriendId);

                FriendSession.WriteComposer(new FriendListUpdateComposer(FriendSession.Character, new Dictionary<int, int>
                { 
                    { 
                        CharacterId, (int) MessengerUpdateType.EnterOrLeave
                    }
                }));
            }
        }

        /// <summary>
        /// Saves some fast-get-requests for the following update round.
        /// </summary>
        /// <param name="CharacterId"></param>
        /// <param name="Mode"> </param>
        public void UpdateRequest(int CharacterId, MessengerUpdateType Mode)
        {
            foreach (int FriendId in GetOnlineMessengerFriends(CharacterId))
            {
                MessengerUpdate Update = new MessengerUpdate {TargetId = FriendId, UpdateId = CharacterId, Mode = Mode};

                UpdateRequests.Add(Update);
            }
        }

        /// <summary>
        /// Returns my friends updates.
        /// </summary>
        /// <param name="CharacterId"></param>
        /// <returns></returns>
        public Dictionary<int, int> GetUpdateRequests(int CharacterId)
        {
            Dictionary<int, int> Output = new Dictionary<int, int>();
            ICollection<MessengerUpdate> ToRemove = new List<MessengerUpdate>();

            foreach (MessengerUpdate Update in UpdateRequests)
            {
                if (Update.TargetId == CharacterId)
                {
                    Output.Add(Update.UpdateId, (int)Update.Mode);
                }
            }

            foreach (MessengerUpdate Update in ToRemove)
            {
                UpdateRequests.Remove(Update);
            }

            return Output;
        }
    }
}
