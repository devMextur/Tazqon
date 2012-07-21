using Tazqon.Packets.Interfaces;
using Tazqon.Network;
using Tazqon.Packets.Composers;
using Tazqon.Habbo.Rooms;
using System.Drawing;

namespace Tazqon.Packets.Events
{
    class MyRoomsSearchMessageEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            Session.WriteComposer(new GuestRoomSearchResultComposer(System.HabboSystem.RoomManager.GetRooms(Session.Character.Id)));
        }
    }

    class QuitMessageEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            RoomAdapter Adapter;

            if (System.HabboSystem.CharacterManager.GetCurrentRoom(Session.Character.ConnectedRoom, out Adapter))
            {
                RoomUnit Unit;

                Point Point = new Point(Adapter.Information.Model.LocationDoorX, Adapter.Information.Model.LocationDoorY);

                if (Adapter.GetUnit(Session.Character.ConnectedRoom, out Unit))
                {
                    Unit.StopWalk();

                    if (Unit.Point == Point)
                    {
                        Unit.Remove();
                        return;
                    }

                    var Path = System.HabboSystem.RoomManager.BlockCalculator.Generate(Unit.Point, Point, Adapter.BlockNodes);

                    if (Path.Count > 0)
                    {
                        if (Path.Count <= 20)
                        {
                            Unit.GivePath(Path);
                        }
                        else
                        {
                            Unit.SetLocation(Point);
                        }
                    }
                    else
                    {
                        Unit.Remove();
                        return;
                    }
                }
            }
        }
    }

    class CanCreateRoomMessageEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            Session.WriteComposer(new CanCreateRoomComposer(false, 10)); // todo
        }
    }

    class CreateFlatMessageEvent : IMessageEvent
    {
        public void Invoke(Session Session, Messages.PacketEvent Packet)
        {
            string Name = Packet.PopString();
            string Model = Packet.PopString();

            System.IOStreamer.AppendLine("Caption: {0}, Model: {1}", Name, Model);
        }
    }

}
