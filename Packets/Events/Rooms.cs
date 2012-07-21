using System.Collections.Generic;

using Tazqon.Packets.Interfaces;
using Tazqon.Network;
using Tazqon.Packets.Composers;
using Tazqon.Habbo.Rooms;
using System.Text;
using System.Drawing;

namespace Tazqon.Packets.Events
{
    class OpenFlatConnectionMessageEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            Room Room;

            if (System.HabboSystem.RoomManager.EnterRoom(Session, Packet.PopInt32(), Packet.PopString(), out Room))
            {
                Session.WriteComposer(new RoomUrlMessageComposer(Room.Id));
                Session.WriteComposer(new RoomReadyMessageComposer(Room.Model.Caption, Room.Id));

                /*
                 * Wallpaper
                 * Floor
                 * Landscape
                 * Rating
                 * Event
                 * */
                Session.Character.UpdateLoadingRoom(Room.Id);
                Session.Character.UpdateConnectedRoom(0);
            }
            else
            {
                Session.WriteComposer(new CloseConnectionMessageComposer());
            }
        }
    }

    class GetHabboGroupBadgesMessageEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            Session.WriteComposer(new HabboGroupBadgesMessageComposer());
        }
    }

    class GetFurnitureAliasesMessageEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            Session.WriteComposer(new FurnitureAliasesMessageComposer());
        }
    }

    class GetRoomEntryDataMessageEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            Room Room;

            if (System.HabboSystem.RoomManager.GetRoom(Session.Character.LoadingRoom, out Room))
            {
                Session.WriteComposer(new HeightMapMessageComposer(Room.Model));
                Session.WriteComposer(new FloorHeightMapMessageComposer(Room.Model));
                Session.Character.UpdateConnectedRoom(Room.Id);
            }
            else
            {
                Session.WriteComposer(new CloseConnectionMessageComposer());
            }
        }
    }

    class GetRoomAdMessageEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            Session.WriteComposer(new RoomAdMessageComposer());
        }
    }

    class GetUserNotificationsMessageEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            Room Room;

            if (System.HabboSystem.RoomManager.GetRoom(Session.Character.LoadingRoom, out Room))
            {
                Session.WriteComposer(new PublicRoomObjectsMessageComposer());
                Session.WriteComposer(new ObjectsMessageComposer());
                Session.WriteComposer(new ItemsMessageComposer());

                RoomUnit Unit;

                if (Room.Adapter.CastPlayer(Session.Character.Id, out Unit))
                {
                    Room.Adapter.WriteComposer(new UsersMessageComposer(new List<RoomUnit> { Unit }));
                    Room.Adapter.WriteComposer(new UserUpdateMessageComposer(new List<RoomUnit> { Unit }));

                    Session.WriteComposer(new UsersMessageComposer(Room.Adapter.Units.Values));
                    // TODO: HideWalls, WallThick, FloorThick
                    Session.WriteComposer(new RoomEntryInfoMessageComposer(Room.Id, Room.GetRoomRight(Session.Character.Id)));
                    Session.WriteComposer(new UserUpdateMessageComposer(Room.Adapter.Units.Values));
                    Session.WriteComposer(new GlobalDanceMessageComposer(Room.Adapter.GetDancingUnits()));

                    // TODO: Rights Composers

                    Unit.Authenticate();
                    Session.Character.UpdateLoadingRoom(0);

                    System.HabboSystem.MessengerManager.UpdateRequest(Session.Character.Id, Habbo.Messenger.MessengerUpdateType.RoomChange);
                }
                else
                {
                    Session.WriteComposer(new CloseConnectionMessageComposer());
                }
            }
        }
    }

    class GetGuestRoomMessageEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            Room Room;

            if (System.HabboSystem.RoomManager.GetRoom(Packet.PopInt32(), out Room))
            {
                Session.WriteComposer(new GetGuestRoomResultComposer(Room, Packet.PopBoolean(), Packet.PopBoolean()));
            }
        }
    }

    class MoveAvatarMessageEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            var Point = new Point(Packet.PopInt32(), Packet.PopInt32());

            RoomAdapter Adapter;

            if (System.HabboSystem.CharacterManager.GetCurrentRoom(Session.Character.Id, out Adapter))
            {
                RoomUnit Unit;

                if (Adapter.GetUnit(Session.Character.Id, out Unit))
                {
                    if (Unit.Point == Point)
                    {
                        return;
                    }

                    Unit.StopWalk();

                    var Path = System.HabboSystem.RoomManager.BlockCalculator.Generate(Unit.Point, Point, Adapter.BlockNodes);

                    if (Path.Count > 0)
                    {
                        Unit.GivePath(Path);
                    }
                }
            }
        }
    }

    class LookToMessageEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            var Point = new Point(Packet.PopInt32(), Packet.PopInt32());

            RoomAdapter Adapter;

            if (System.HabboSystem.CharacterManager.GetCurrentRoom(Session.Character.Id, out Adapter))
            {
                RoomUnit Unit;

                if (Adapter.GetUnit(Session.Character.Id, out Unit))
                {
                    if (Unit.Point == Point)
                    {
                        return;
                    }

                    Unit.StopWalk();

                    Unit.LookAt(Point);
                }
            }
        }
    }

    class StartTypingMessageEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            RoomAdapter Adapter;

            if (System.HabboSystem.CharacterManager.GetCurrentRoom(Session.Character.Id, out Adapter))
            {
                RoomUnit Unit;

                if (Adapter.GetUnit(Session.Character.Id, out Unit))
                {
                    Adapter.WriteComposer(new UserTypingMessageComposer(Unit.Id, true));
                }
            }
        }
    }

    class CancelTypingMessageEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            RoomAdapter Adapter;

            if (System.HabboSystem.CharacterManager.GetCurrentRoom(Session.Character.Id, out Adapter))
            {
                RoomUnit Unit;

                if (Adapter.GetUnit(Session.Character.Id, out Unit))
                {
                    Adapter.WriteComposer(new UserTypingMessageComposer(Unit.Id, false));
                }
            }
        }
    }

    class ChatMessageEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            RoomAdapter Adapter;

            if (System.HabboSystem.CharacterManager.GetCurrentRoom(Session.Character.Id, out Adapter))
            {
                RoomUnit Unit;

                if (Adapter.GetUnit(Session.Character.Id, out Unit))
                {
                    Adapter.WriteComposer(new ChatMessageComposer(Unit.Id, Packet.PopString()));
                }
            }
        }
    }

    class ShoutMessageEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            RoomAdapter Adapter;

            if (System.HabboSystem.CharacterManager.GetCurrentRoom(Session.Character.Id, out Adapter))
            {
                RoomUnit Unit;

                if (Adapter.GetUnit(Session.Character.Id, out Unit))
                {
                    Adapter.WriteComposer(new ShoutMessageComposer(Unit.Id, Packet.PopString()));
                }
            }
        }
    }

    class DanceMessageEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            RoomAdapter Adapter;

            if (System.HabboSystem.CharacterManager.GetCurrentRoom(Session.Character.Id, out Adapter))
            {
                RoomUnit Unit;

                if (Adapter.GetUnit(Session.Character.Id, out Unit))
                {
                    var DanceId = Packet.PopInt32();

                    Unit.Dance(DanceId);
                    Unit.Activate();

                    Adapter.WriteComposer(new DanceMessageComposer(Unit.Id, DanceId));
                }
            }
        }
    }

    class WaveMessageEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            RoomAdapter Adapter;

            if (System.HabboSystem.CharacterManager.GetCurrentRoom(Session.Character.Id, out Adapter))
            {
                RoomUnit Unit;

                if (Adapter.GetUnit(Session.Character.Id, out Unit))
                {
                    Unit.Activate();
                    Adapter.WriteComposer(new WaveMessageComposer(Unit.Id));
                }
            }
        }
    }
}
