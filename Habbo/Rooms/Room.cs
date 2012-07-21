using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Tazqon.Commons.Adapters;

namespace Tazqon.Habbo.Rooms
{
    class Room
    {
        /// <summary>
        /// Id of the room.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Caption of the room.
        /// </summary>
        public string Caption { get; private set; }

        /// <summary>
        /// Description of the room,
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// CharacterId of the room.
        /// </summary>
        public int CharacterId { get; private set; }

        /// <summary>
        /// Id of the model.
        /// </summary>
        public int ModelId { get; private set; }

        /// <summary>
        /// Room Model of the room.
        /// </summary>
        public RoomModel Model
        {
            get { return System.HabboSystem.RoomManager.GetModel(ModelId); }
        }

        /// <summary>
        /// Doorstate of the room.
        /// </summary>
        public DoorState DoorState { get; private set; }

        /// <summary>
        /// Password of the room.
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Checks if theres an adapter running.
        /// </summary>
        public bool HasAdapter
        {
            get
            {
                return System.HabboSystem.RoomManager.HasAdapter(Id);
            }
        }

        /// <summary>
        /// Gets the current adapter that runs.
        /// </summary>
        public RoomAdapter Adapter
        {
            get
            {
                return System.HabboSystem.RoomManager.CastAdapter(Id);
            }
        }

        /// <summary>
        /// Characters with rights.
        /// </summary>
        public ICollection<int> Rights { get; private set; }

        public Room(DataRow Row)
        {
            using (RowAdapter Adapter = new RowAdapter(Row))
            {
                this.Id = Adapter.PopInt32("id");
                this.Caption = Adapter.PopString("caption");
                this.Description = Adapter.PopString("description");
                this.CharacterId = Adapter.PopInt32("character_id");
                this.ModelId = Adapter.PopInt32("model_id");
                this.DoorState = Adapter.PopEnum<DoorState>("door_state");
                this.Password = Adapter.PopString("password");
            }
        }

        /// <summary>
        /// Gets the rights for an specified characterid.
        /// </summary>
        /// <param name="CharacterId"></param>
        /// <returns></returns>
        public RoomRight GetRoomRight(int CharacterId)
        {
            if (this.CharacterId == CharacterId)
            {
                return RoomRight.Owner;
            }

            if (Rights == null)
            {
                // TODO: LOAD RIGHTS
            }

            return RoomRight.None;
        }
    }

    enum DoorState
    {
        Open = 0, Doorbell = 1, Password = 2
    }

    enum RoomRight
    {
        None = 0, Rights = 1, Owner = 2
    }
}
