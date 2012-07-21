using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using Tazqon.Commons.Adapters;
using Tazqon.Habbo.Pathfinding;
using Tazqon.Network;
using Tazqon.Packets.Composers;
using Tazqon.Storage.Querys;

namespace Tazqon.Habbo.Rooms
{
    class RoomManager
    {
        /// <summary>
        /// Storage of the models of rooms.
        /// </summary>
        public Dictionary<int, RoomModel> Models { get; private set; }

        /// <summary>
        /// Storage of progressing rooms.
        /// </summary>
        public Dictionary<int, RoomAdapter> Adapters { get; private set; }

        /// <summary>
        /// Cache to buffer some sql data in.
        /// </summary>
        public Dictionary<int, Room> WeakSQLCache { get; private set; }

        /// <summary>
        /// Calculator for walking etc.
        /// </summary>
        public BlockCalculator BlockCalculator { get; private set; }

        /// <summary>
        /// Handler of alive things.
        /// </summary>
        public Timer RoomProgressTimer { get; private set; }

        public RoomManager()
        {
            Models = new Dictionary<int, RoomModel>();
            Adapters = new Dictionary<int, RoomAdapter>();
            WeakSQLCache = new Dictionary<int, Room>();
            BlockCalculator = new BlockCalculator();
            RoomProgressTimer = new Timer(HandleTimer, RoomProgressTimer, 0, 530);

            foreach (DataRow Row in System.MySQLManager.GetObject(new RoomModelsQuery()).GetOutput<DataTable>().Rows)
            {
                RoomModel Model = new RoomModel(Row);

                if (!Models.ContainsKey(Model.Id))
                {
                    Models.Add(Model.Id, Model);
                }
            }
        }

        public void HandleTimer(object Obj)
        {
            foreach (RoomAdapter Adapter in Adapters.Values)
            {
                ICollection<RoomUnit> Units;

                if (Adapter.GetUnitUpdates(out Units))
                {
                    ICollection<RoomUnit> UnitsToSerialize = new List<RoomUnit>();

                    foreach (RoomUnit Unit in Units)
                    {
                        if (Unit.WalkOneTile() || Unit.NeedsUpdate)
                        { 
                            UnitsToSerialize.Add(Unit);
                        }
                    }

                    Adapter.WriteComposer(new UserUpdateMessageComposer(UnitsToSerialize));
                }

                ICollection<RoomUnit> KickUnits;

                if (Adapter.GetUnitsToKick(out KickUnits))
                {
                    foreach (RoomUnit Unit in KickUnits)
                    {
                        if (Adapter.RemovePlayerByUnitId(Unit.Id))
                        {
                            Unit.GetSession().WriteComposer(new CloseConnectionMessageComposer());
                            Adapter.WriteComposer(new UserRemoveMessageComposer(Unit.Id));
                        }
                    }
                }
            }
        }

        public RoomModel GetModel(int Id)
        {
            using (DictionaryAdapter<int, RoomModel> DA = new DictionaryAdapter<int, RoomModel>(Models))
            {
                return DA.TryPopValue(Id);
            }
        }

        public Room GetRoom(int RoomId)
        {
            RoomAdapter Adapter;
            Room Room;

            if (!Adapters.TryGetValue(RoomId, out Adapter))
            {
                if (!WeakSQLCache.TryGetValue(RoomId, out Room))
                {
                    Room = new Room(System.MySQLManager.GetObject(new RoomInfoQuery(RoomId)).GetOutput<DataRow>());
                    WeakSQLCache.Add(RoomId, Room);
                }
            }
            else Room = Adapter.Information;

            return Room;
        }

        public bool GetRoom(int RoomId, out Room Room)
        {
            Room = GetRoom(RoomId);

            return Room != null;
        }

        public bool HasAdapter(int RoomId)
        {
            return Adapters.ContainsKey(RoomId);
        }

        public RoomAdapter CastAdapter(int RoomId)
        {
            RoomAdapter Adapter;

            if (!Adapters.TryGetValue(RoomId, out Adapter))
            {
                Adapter = new RoomAdapter(GetRoom(RoomId));
                Adapters.Add(Adapter.Information.Id, Adapter);

                if (WeakSQLCache.ContainsKey(RoomId))
                {
                    WeakSQLCache.Remove(RoomId);
                }
            }

            return Adapter;
        }

        public void DisposeAdapter(RoomAdapter Adapter)
        {
            if (!WeakSQLCache.ContainsKey(Adapter.Information.Id))
            {
                WeakSQLCache.Add(Adapter.Information.Id, Adapter.Information);
            }
            else WeakSQLCache[Adapter.Information.Id] = Adapter.Information;

            Adapters.Remove(Adapter.Information.Id);

            Adapter.Dispose();
        }


        public ICollection<Room> GetRooms(int CharacterId)
        {
            ICollection<Room> Output = new List<Room>();

            foreach(DataRow Row in System.MySQLManager.GetObject(new RoomsQuery(CharacterId)).GetOutput<DataTable>().Rows)
            {
                using (RowAdapter Adapter = new RowAdapter(Row))
                {
                    Room Room = GetRoom(Adapter.PopInt32("id"));

                    Output.Add(Room);
                }
            }

            return Output;
        }

        public bool EnterRoom(Session Session,int RoomId, string Password, out Room Room)
        {
            Room = System.HabboSystem.RoomManager.GetRoom(RoomId);

            if (Room == null)
            {
                return false;
            }

            if (Room.GetRoomRight(Session.Character.Id) != RoomRight.Owner)
            {
                if (Room.DoorState == DoorState.Password)
                {
                    if (Room.Password != Password)
                    {
                        Session.WriteComposer(new GenericErrorComposer(-100002));
                        Session.WriteComposer(new CloseConnectionMessageComposer());
                        return false;
                    }
                }
            }

            RoomAdapter Adapter = System.HabboSystem.RoomManager.CastAdapter(RoomId);

            if (System.HabboSystem.RoomManager.HasAdapter(RoomId))
            {
                if (Room.GetRoomRight(Session.Character.Id) != RoomRight.Owner)
                {
                    if (Adapter.PlayersAmount >= Adapter.Information.Model.MaximalUnits)
                    {
                        Session.WriteComposer(new CantConnectMessageComposer(1));
                        Session.WriteComposer(new CloseConnectionMessageComposer());
                        return false;
                    }
                }
            }

            Session.WriteComposer(new OpenConnectionMessageComposer());

            return true;
        }
    }

    enum NavigatorTab
    {
        PRIVATE_ITEMS = 1, RATED_ITEMS = 2, MY_RAND_FRIENDS = 3, MY_ALIVE_FRIENDS = 4,
        MY_ITEMS = 5, MY_FAV_ITEMS = 6, MY_VISITED_ITEMS = 7, SEARCHED_ITEMS = 8
    }
}
