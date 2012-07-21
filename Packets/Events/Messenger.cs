using System.Collections.Generic;
using Tazqon.Commons.Adapters;
using Tazqon.Packets.Interfaces;
using Tazqon.Packets.Composers;
using Tazqon.Habbo.Characters;
using Tazqon.Storage.Querys;
using Tazqon.Habbo.Messenger;
using System.Data;

namespace Tazqon.Packets.Events
{
    class MessengerInitMessageEvent : IMessageEvent
    {
        public void Invoke(Network.Session Session, Messages.PacketEvent Packet)
        {
            Session.Character.UpdateMessengerGroups(System.HabboSystem.MessengerManager.GetMessengerGroups(Session.Character.Id));
            Session.Character.UpdateMessengerFriends(System.HabboSystem.MessengerManager.GetMessengerFriends(Session.Character.Id));
            Session.WriteComposer(new MessengerInitComposer(System.HabboSystem.MessengerManager.MAX_FRIENDS_DEFAULT, Session.Character));
        }
    }

    class GetBuddyRequestsMessageEvent : IMessageEvent
    {
        public void Invoke(Network.Session Session, Messages.PacketEvent Packet)
        {
            Session.Character.UpdateMessengerRequests(System.HabboSystem.MessengerManager.GetMessengerRequests(Session.Character.Id));
            Session.WriteComposer(new BuddyRequestsComposer(Session.Character.MessengerRequests));
        }
    }

    class FriendListUpdateMessageEvent : IMessageEvent
    {
        public void Invoke(Network.Session Session, Messages.PacketEvent Packet)
        {
            Dictionary<int,int> UpdateRequests = System.HabboSystem.MessengerManager.GetUpdateRequests(Session.Character.Id);
            Session.WriteComposer(new FriendListUpdateComposer(Session.Character, UpdateRequests));
        }
    }

    class SendMsgMessageEvent : IMessageEvent
    {
        public void Invoke(Network.Session Session, Messages.PacketEvent Packet)
        {
            // TODO IF(BUSY)

            var TargetId = Packet.PopInt32();

             /* ChatErrorId(s)
             * 3 = Friend Muted x
             * 4 = You are muted x
             * 5 = offline x
             * 6 = not friend x 
             * 7 = friend is busy 
             * */

            if (TargetId < 1 || TargetId == Session.Character.Id)
            {
                return;
            }

            if (Session.Character.Muted)
            {
                Session.WriteComposer(new InstantMessageErrorComposer(4, TargetId));
                return;
            }

            if (!Session.Character.MessengerFriends.Contains(TargetId))
            {
                Session.WriteComposer(new InstantMessageErrorComposer(6, TargetId));
                return;
            }

            if (System.HabboSystem.CharacterManager.GetStatus(TargetId) == CharacterStatus.Offline)
            {
                Session.WriteComposer(new InstantMessageErrorComposer(5, TargetId));
                return;
            }

            var TargetSession = System.NetworkSocket.GetSession(TargetId);

            if (TargetSession == null)
            {
                Session.WriteComposer(new InstantMessageErrorComposer(5, TargetId));
                return;
            }

            if (TargetSession.Character.Muted)
            {
                Session.WriteComposer(new InstantMessageErrorComposer(3, TargetId));
                return;
            }

            TargetSession.WriteComposer(new NewConsoleMessageComposer(Session.Character.Id, Packet.PopString()));
        }
    }

    class SendRoomInviteMessageEvent : IMessageEvent
    {
        public void Invoke(Network.Session Session, Messages.PacketEvent Packet)
        {
            // TODO ? IF(!INROOM)

            if (Session.Character.Muted)
            {
                Session.WriteComposer(new ModMessageComposer("You are muted, only someone with an higher rank can unmute you."));
                return;
            }

            ICollection<int> Targets = Packet.PopCollection();

            string Message = Packet.PopString();

            foreach (int TargetId in Targets)
            {
                if (TargetId < 1 || TargetId == Session.Character.Id)
                {
                    continue;
                }

                if (System.HabboSystem.CharacterManager.GetStatus(TargetId) == CharacterStatus.Offline)
                {
                    continue;
                }

                if (!Session.Character.MessengerFriends.Contains(TargetId))
                {
                    Session.WriteComposer(new RoomInviteErrorComposer());
                    continue;
                }

                var TargetSession = System.NetworkSocket.GetSession(TargetId);

                if (TargetSession == null)
                {
                    Session.WriteComposer(new RoomInviteErrorComposer());
                    continue;
                }

                if (TargetSession.Character.Muted)
                {
                    Session.WriteComposer(new ModMessageComposer(string.Format("{0} is muted.", Session.Character.Username)));
                    continue;
                }

                TargetSession.WriteComposer(new RoomInviteComposer(Session.Character.Id, Message));
            }
        }
    }

    class RemoveBuddyMessageEvent : IMessageEvent
    {
        public void Invoke(Network.Session Session, Messages.PacketEvent Packet)
        {
            foreach (int TargetId in Packet.PopCollection())
            {
                if (TargetId < 1 || TargetId == Session.Character.Id)
                {
                    continue;
                }

                if (!Session.Character.MessengerFriends.Contains(TargetId))
                {
                    continue;
                }

                CharacterStatus Status = System.HabboSystem.CharacterManager.GetStatus(TargetId);

                Session.WriteComposer(new FriendListUpdateComposer(Session.Character, new Dictionary<int, int>
                                                                                          { 
                    { 
                        TargetId, (int)MessengerUpdateType.RemoveBuddy
                    }
                }));

                if (Status == CharacterStatus.Online)
                {
                    var TargetSession = System.NetworkSocket.GetSession(TargetId);

                    if (TargetSession != null)
                    {
                        TargetSession.WriteComposer(new FriendListUpdateComposer(TargetSession.Character, new Dictionary<int, int>
                        { 
                            { 
                                Session.Character.Id, (int)MessengerUpdateType.RemoveBuddy
                            }
                        }));

                        TargetSession.Character.MessengerFriends.Remove(Session.Character.Id);
                    }
                }

                Session.Character.MessengerFriends.Remove(TargetId);

                System.MySQLManager.InvokeQuery(new MessengerFriendRemoveQuery(Session.Character.Id, TargetId));
            }
        }
    }

    class HabboSearchMessageEvent : IMessageEvent
    {
        public void Invoke(Network.Session Session, Messages.PacketEvent Packet)
        {
            string Username = Packet.PopString();

            ICollection<int> Friends = new List<int>();
            ICollection<int> NoFriends = new List<int>();

            foreach (DataRow Row in System.MySQLManager.GetObject(new MessengerCharacterSearchQuery(Username)).GetOutput<DataTable>().Rows)
            {
                using (RowAdapter Adapter = new RowAdapter(Row))
                {
                    int CharacterId = Adapter.PopInt32("id");

                    if (Session.Character.MessengerFriends.Contains(CharacterId))
                    {
                        Friends.Add(CharacterId);
                    }
                    else NoFriends.Add(CharacterId);
                }
            }

            Session.WriteComposer(new HabboSearchResultComposer(Session.Character, Friends, NoFriends));
        }
    }

    class AcceptBuddyMessageEvent : IMessageEvent
    {
        public void Invoke(Network.Session Session, Messages.PacketEvent Packet)
        {
            foreach (int TargetId in Packet.PopCollection())
            {
                if (TargetId < 1 || TargetId == Session.Character.Id)
                {
                    continue;
                }

                if (Session.Character.MessengerFriends.Contains(TargetId))
                {
                    continue;
                }

                if (!Session.Character.MessengerRequests.Contains(TargetId))
                {
                    continue;
                }

                if (Session.Character.MessengerFriends.Count >= System.HabboSystem.MessengerManager.GetLimitFriends(Session.Character.Id))
                {
                    Session.WriteComposer(new MessengerErrorComposer(39, 2));
                    return;
                }

                Session.Character.MessengerFriends.Add(TargetId);
                Session.Character.MessengerRequests.Remove(TargetId);

                CharacterStatus Status = System.HabboSystem.CharacterManager.GetStatus(TargetId);

                Session.WriteComposer(new FriendListUpdateComposer(Session.Character, new Dictionary<int, int>
                { 
                    { 
                        TargetId, (int)MessengerUpdateType.NewFriendship
                    }
                }));

                if (Status == CharacterStatus.Online)
                {
                    var TargetSession = System.NetworkSocket.GetSession(TargetId);

                    if (TargetSession != null)
                    {
                        TargetSession.WriteComposer(new FriendListUpdateComposer(TargetSession.Character, new Dictionary<int, int> 
                        { 
                            { 
                                Session.Character.Id, (int)MessengerUpdateType.NewFriendship
                            }
                        }));

                        TargetSession.Character.MessengerFriends.Add(Session.Character.Id);
                    }
                }

                System.MySQLManager.InvokeQuery(new MessengerAcceptFriendQuery(TargetId, Session.Character.Id));
            }
        }
    }

    class DeclineBuddyMessageEvent : IMessageEvent
    {
        public void Invoke(Network.Session Session, Messages.PacketEvent Packet)
        {
            bool RemoveAll = Packet.PopBoolean();

            if (RemoveAll)
            {
                Session.Character.MessengerRequests.Clear();
                System.MySQLManager.InvokeQuery(new MessengerDeclineAllFriendQuery(Session.Character.Id));
            }
            else
            {
                foreach (int TargetId in Packet.PopCollection())
                {
                    if (TargetId < 1 || TargetId == Session.Character.Id)
                    {
                        continue;
                    }

                    if (!Session.Character.MessengerRequests.Contains(TargetId))
                    {
                        continue;
                    }

                    Session.Character.MessengerRequests.Remove(TargetId);
                    System.MySQLManager.InvokeQuery(new MessengerDeclineFriendQuery(TargetId, Session.Character.Id));
                }
            }
        }
    }

    class RequestBuddyMessageEvent : IMessageEvent
    {
        public void Invoke(Network.Session Session, Messages.PacketEvent Packet)
        {
            int TargetId = System.HabboSystem.CharacterManager.GetId(Packet.PopString());

            if (Session.Character.MessengerFriends.Count >= System.HabboSystem.MessengerManager.GetLimitFriends(Session.Character.Id))
            {
                Session.WriteComposer(new MessengerErrorComposer(39, 2));
                return;
            }

            if (TargetId < 1 || TargetId == Session.Character.Id)
            {
                return;
            }

            if (Session.Character.MessengerRequests.Contains(TargetId))
            {
                return;
            }

            if (Session.Character.MessengerFriends.Contains(TargetId))
            {
                return;
            }

            if (!System.HabboSystem.CharacterManager.GetAllowNewFriends(TargetId))
            {
                Session.WriteComposer(new MessengerErrorComposer(39, 3));
                return;
            }

            CharacterStatus Status = System.HabboSystem.CharacterManager.GetStatus(TargetId);

            bool Flag = false;

            if (Status == CharacterStatus.Online)
            {
                var TargetSession = System.NetworkSocket.GetSession(TargetId);

                if (TargetSession == null)
                {
                    return;
                }

                if (TargetSession.Character.MessengerFriends.Count >= System.HabboSystem.MessengerManager.GetLimitFriends(TargetSession.Character.Id))
                {
                    Session.WriteComposer(new MessengerErrorComposer(39, 2));
                    return;
                }

                if (!TargetSession.Character.MessengerRequests.Contains(Session.Character.Id))
                {
                    TargetSession.Character.MessengerRequests.Add(Session.Character.Id);

                    TargetSession.WriteComposer(new NewBuddyRequestComposer(Session.Character.Id, Session.Character.Username, Session.Character.Figure));
                    Flag = true;
                }
            }
            else
            {
                if (System.HabboSystem.MessengerManager.GetMessengerFriends(TargetId).Count >= System.HabboSystem.MessengerManager.GetLimitFriends(TargetId))
                {
                    Session.WriteComposer(new MessengerErrorComposer(39, 2));
                    return;
                }

                if (!System.HabboSystem.MessengerManager.GetMessengerRequests(TargetId).Contains(Session.Character.Id))
                {
                    Flag = true;
                }
            }

            if (Flag)
            {
                System.MySQLManager.InvokeQuery(new MessengerRequestFriendQuery(Session.Character.Id, TargetId));
            }
        }
    }

    class FollowFriendMessageEvent : IMessageEvent
    {
        public void Invoke(Network.Session Session, Messages.PacketEvent Packet)
        {
            var FriendId = Packet.PopInt32();

            if (!Session.Character.MessengerFriends.Contains(FriendId))
            {
                return;
            }

            var RoomId = System.HabboSystem.CharacterManager.GetCurrentRoom(FriendId);

            Session.WriteComposer(new RoomForwardMessageComposer(RoomId));
        }
    }
}
