using Tazqon.Packets.Messages;
using Tazqon.Packets.Headers;
using Tazqon.Habbo.Rooms;
using System.Collections.Generic;
using System;

namespace Tazqon.Packets.Composers
{
    class GuestRoomSearchResultComposer : PacketComposer
    {
        public GuestRoomSearchResultComposer(ICollection<Room> Rooms, string Param = "", bool Events = false, int CatId = -1, NavigatorTab Tab = NavigatorTab.MY_ITEMS)
        {
            base.WriteHeader(MessageComposerIds.GuestRoomSearchResultComposer);
            base.Write(CatId > 0 ? CatId : (int)Tab);
            base.Write(CatId > 0 ? CatId + string.Empty : Param);
            base.Write(Rooms.Count);

            foreach (Room Room in Rooms)
            {
                base.Write(new TazqonRoomInformationComposer(Room, Events));
            }
        }
    }

    class TazqonRoomInformationComposer : PacketComposer
    {
        public TazqonRoomInformationComposer(Room Room, bool Events)
        {
            base.Write(Room.Id);
            base.Write(Events);
            base.Write(Room.Caption);
            base.Write(System.HabboSystem.CharacterManager.GetUsername(Room.CharacterId));
            base.Write((int)Room.DoorState);
            base.Write(Room.HasAdapter ? Room.Adapter.PlayersAmount : 0);
            base.Write(Room.Model.MaximalUnits);
            base.Write(Room.Description);
            base.Write(0); // ??
            base.Write(true); // enable trade
            base.Write(0); // rating
            base.Write(0);// catid
            base.Write(string.Empty);
            base.Write(0); // tag count

            //Array.ForEach(Item.Tags.ToArray(), Message.Append);

            base.Write(0); // Bg icon
            base.Write(0); // fg icon
            base.Write(0); // icon amount (int, int)

            base.Write(false); // allopets
            base.Write(false); // allopetseat
        }
    }

    class CanCreateRoomComposer : PacketComposer
    {
        public CanCreateRoomComposer(bool NotAllowed, int Limit)
        {
            base.WriteHeader(MessageComposerIds.CanCreateRoomComposer);
            base.Write(NotAllowed);
            base.Write(Limit);  
        }
    }

}
