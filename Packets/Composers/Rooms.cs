using Tazqon.Packets.Messages;
using Tazqon.Packets.Headers;
using Tazqon.Habbo.Rooms;
using System.Collections.Generic;
using System.Text;

namespace Tazqon.Packets.Composers
{
    class CantConnectMessageComposer : PacketComposer
    {
        public CantConnectMessageComposer(int ErrorCode)
        {
            base.WriteHeader(MessageComposerIds.CantConnectMessageComposer);
            base.Write(ErrorCode);
        }
    }

    class CloseConnectionMessageComposer : PacketComposer
    {
        public CloseConnectionMessageComposer()
        {
            base.WriteHeader(MessageComposerIds.CloseConnectionMessageComposer);
        }
    }

    class OpenConnectionMessageComposer : PacketComposer
    {
        public OpenConnectionMessageComposer()
        {
            base.WriteHeader(MessageComposerIds.OpenConnectionMessageComposer);
        }
    }

    class RoomUrlMessageComposer : PacketComposer
    {
        public RoomUrlMessageComposer(int RoomId)
        {
            base.WriteHeader(MessageComposerIds.RoomUrlMessageComposer);
            base.Write(string.Format("/client/private/{0}/id", RoomId));
        }
    }

    class RoomReadyMessageComposer : PacketComposer
    {
        public RoomReadyMessageComposer(string Model, int RoomId)
        {
            base.WriteHeader(MessageComposerIds.RoomReadyMessageComposer);
            base.Write(Model);
            base.Write(RoomId);
        }
    }

    class HabboGroupBadgesMessageComposer : PacketComposer
    {
        public HabboGroupBadgesMessageComposer()
        {
            base.WriteHeader(MessageComposerIds.HabboGroupBadgesMessageComposer);
            base.Write(0); // Badge amount {}
        }
    }

    class FurnitureAliasesMessageComposer : PacketComposer
    {
        public FurnitureAliasesMessageComposer()
        {
            base.WriteHeader(MessageComposerIds.FurnitureAliasesMessageComposer);
            base.Write(0); // Furni amount {}
        }
    }

    class HeightMapMessageComposer : PacketComposer
    {
        public HeightMapMessageComposer(RoomModel Model)
        {
            base.WriteHeader(MessageComposerIds.HeightMapMessageComposer);
            base.Write(Model.ParametersWithOutDoor);
        }
    }

    class FloorHeightMapMessageComposer : PacketComposer
    {
        public FloorHeightMapMessageComposer(RoomModel Model)
        {
            base.WriteHeader(MessageComposerIds.FloorHeightMapMessageComposer);
            base.Write(Model.ParametersWithDoor);
        }
    }

    class RoomAdMessageComposer : PacketComposer
    {
        public RoomAdMessageComposer()
        {
            base.WriteHeader(MessageComposerIds.RoomAdMessageComposer);
        }
    }

    class PublicRoomObjectsMessageComposer : PacketComposer
    {
        public PublicRoomObjectsMessageComposer()
        {
            base.WriteHeader(MessageComposerIds.PublicRoomObjectsMessageComposer);
            base.Write(false); // TODO: PUBLIC ROOM
        }
    }

    class ObjectsMessageComposer : PacketComposer
    {
        public ObjectsMessageComposer()
        {
            base.WriteHeader(MessageComposerIds.ObjectsMessageComposer);
            base.Write(0); // WallFurni amount {}
        }
    }

    class ItemsMessageComposer : PacketComposer
    {
        public ItemsMessageComposer()
        {
            base.WriteHeader(MessageComposerIds.ItemsMessageComposer);
            base.Write(0); // WallFurni amount {}
        }
    }

    class UsersMessageComposer : PacketComposer
    {
        public UsersMessageComposer(ICollection<RoomUnit> Units)
        {
            base.WriteHeader(MessageComposerIds.UsersMessageComposer);
            base.Write(Units.Count);

            foreach (RoomUnit Unit in Units)
            {
                base.Write(Unit.BaseId);

                if (Unit.Type == UnitType.Player)
                {
                    base.Write(Unit.GetSession().Character.Username);
                    base.Write(Unit.GetSession().Character.Motto);
                    base.Write(Unit.GetSession().Character.Figure);
                }

                base.Write(Unit.Id);
                base.Write(Unit.X);
                base.Write(Unit.Y);
                base.Write("0.0");

                if (Unit.Type == UnitType.Player)
                {
                    base.Write(2);
                    base.Write(1);
                    base.Write(Unit.GetSession().Character.Gender.ToString().ToLower());
                    base.Write(-1);
                    base.Write(-1);
                    base.Write(-1);
                    base.Write(string.Empty);
                    base.Write(System.HabboSystem.AchievementManager.GetAchievementScore(Unit.GetSession().Character));
                }
            }
        }
    }

    class RoomEntryInfoMessageComposer : PacketComposer
    {
        public RoomEntryInfoMessageComposer(int RoomId, RoomRight Right)
        {
            base.WriteHeader(MessageComposerIds.RoomEntryInfoMessageComposer);
            base.Write(true); // PRIVATEROOM
            base.Write(RoomId);
            base.Write(Right == RoomRight.Owner);
        }
    }

    class UserUpdateMessageComposer : PacketComposer
    {
        public UserUpdateMessageComposer(ICollection<RoomUnit> Units)
        {
            base.WriteHeader(MessageComposerIds.UserUpdateMessageComposer);
            base.Write(Units.Count);

            foreach (RoomUnit Unit in Units)
            {
                base.Write(Unit.Id);
                base.Write(Unit.X);
                base.Write(Unit.Y);
                base.Write(Unit.Z.ToString().Replace(',', '.'));
                base.Write(Unit.Rotation);
                base.Write(Unit.Rotation);

                StringBuilder ActivitiesBuilder = new StringBuilder("/");

                foreach (KeyValuePair<string, string> kvp in Unit.Activities)
                {
                    ActivitiesBuilder.Append(string.Format("{0} {1}/", kvp.Key, kvp.Value));
                }

                ActivitiesBuilder.Append('/');

                base.Write(ActivitiesBuilder.ToString());
            }
        }
    }

    class GetGuestRoomResultComposer : PacketComposer
    {
        public GetGuestRoomResultComposer(Room Room, bool LoadingState, bool Following)
        {
            base.WriteHeader(MessageComposerIds.GetGuestRoomResultComposer);
            base.Write(LoadingState);
            base.Write(new TazqonRoomInformationComposer(Room, false));
            base.Write(Following);
            base.Write(LoadingState);
        }
    }

    class RoomForwardMessageComposer : PacketComposer
    {
        public RoomForwardMessageComposer(int RoomId)
        {
            base.WriteHeader(MessageComposerIds.RoomForwardMessageComposer);
            base.Write(0);
            base.Write(RoomId);
        }
    }

    class UserRemoveMessageComposer : PacketComposer
    {
        public UserRemoveMessageComposer(int UnitId)
        {
            base.WriteHeader(MessageComposerIds.UserRemoveMessageComposer);
            base.Write(UnitId + string.Empty);
        }
    }

    class UserTypingMessageComposer : PacketComposer
    {
        public UserTypingMessageComposer(int UnitId, bool Typing)
        {
            base.WriteHeader(MessageComposerIds.UserTypingMessageComposer);
            base.Write(UnitId);
            base.Write(Typing);
        }
    }

    class ChatMessageComposer : PacketComposer
    {
        public ChatMessageComposer(int UnitId, string Text)
        {
            base.WriteHeader(MessageComposerIds.ChatMessageComposer);
            base.Write(UnitId);
            base.Write(Text);
            base.Write(0); // Emoticon
            base.Write(0); // Links
        }
    }

    class ShoutMessageComposer : PacketComposer
    {
        public ShoutMessageComposer(int UnitId, string Text)
        {
            base.WriteHeader(MessageComposerIds.ShoutMessageComposer);
            base.Write(UnitId);
            base.Write(Text);
            base.Write(0); // Emoticon
            base.Write(0); // Links
        }
    }

    class DanceMessageComposer : PacketComposer
    {
        public DanceMessageComposer(int UnitId, int DanceId)
        {
            base.WriteHeader(MessageComposerIds.DanceMessageComposer);
            base.Write(UnitId);
            base.Write(DanceId);
        }
    }

    class GlobalDanceMessageComposer : PacketComposer
    {
        public GlobalDanceMessageComposer(ICollection<RoomUnit> Units)
        {
            foreach (RoomUnit Unit in Units)
            {
                base.WriteHeader(MessageComposerIds.DanceMessageComposer);
                base.Write(Unit.Id);
                base.Write(Unit.CurrentDanceId);
            }
        }
    }

    class WaveMessageComposer : PacketComposer
    {
        public WaveMessageComposer(int Unitd)
        {
            base.WriteHeader(MessageComposerIds.WaveMessageComposer);
            base.Write(Unitd);
        }
    }
}
