using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using Tazqon.Commons.Adapters;
using Tazqon.Commons.Components;
using Tazqon.Habbo.Pathfinding;
using Tazqon.Packets.Messages;

namespace Tazqon.Habbo.Rooms
{
    class RoomAdapter : IDisposable
    {
        /// <summary>
        /// Information of the room. (Memory)
        /// </summary>
        public Room Information { get; private set; }

        /// <summary>
        /// Amount of (REAL) players.
        /// </summary>
        public int PlayersAmount
        {
            get { return (from item in Units.Values where item.Type == UnitType.Player where item.Authenticated select item).Count(); }
        }

        /// <summary>
        /// Returns an collection with specified type.
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public ICollection<RoomUnit> GetUnits(UnitType Type)
        {
            return (from item in Units.Values where item.Type == Type where item.Authenticated select item).ToList();
        }

        /// <summary>
        /// Returns an collection with authenticated units
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public ICollection<RoomUnit> GetUnits()
        {
            return (from item in Units.Values where item.Authenticated select item).ToList();
        }

        /// <summary>
        /// Returns an collection with authenticated units
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public ICollection<RoomUnit> GetDancingUnits()
        {
            return (from item in Units.Values where item.CurrentDanceId > 0 select item).ToList();
        }

        /// <summary>
        /// Units who are running/walking in the room.
        /// </summary>
        public Dictionary<int, RoomUnit> Units { get; private set; }

        /// <summary>
        /// Handles fawlessly incoming/outgoing units.
        /// </summary>
        public ReaderWriterLockSlim UnitLocker { get; private set; }

        /// <summary>
        /// Gives every unit an id.
        /// </summary>
        public SafeInteger UnitCounter { get; private set; }

        /// <summary>
        /// Nodes of the tiles.
        /// </summary>
        public ICollection<BlockNode> BlockNodes { get; private set; }

        public RoomAdapter(Room Information)
        {
            this.Information = Information;
            this.Units = new Dictionary<int, RoomUnit>();
            this.UnitLocker = new ReaderWriterLockSlim();
            this.UnitCounter = new SafeInteger(0, true);

            this.BlockNodes = new List<BlockNode>();

            foreach (TileNode Node in Information.Model.Nodes)
            {
                var Point = new Point(Node.IX, Node.IY);

                var State = Node.DeadTile ? BlockState.BLOCKED : BlockState.OPEN;

                if (Point == new Point(Information.Model.LocationDoorX, Information.Model.LocationDoorY))
                {
                    State = BlockState.OPEN_LAST_STEP;
                }

                var Block = new BlockNode(Point, Node.Height, State);

                this.BlockNodes.Add(Block);
            }
        }

        /// <summary>
        /// Created and adds an new player.
        /// </summary>
        /// <param name="BaseId"></param>
        /// <param name="Unit"></param>
        /// <returns></returns>
        public bool CastPlayer(int BaseId, out RoomUnit Unit)
        {
            Unit = new RoomUnit(UnitCounter.Push(), BaseId, UnitType.Player,
                new Point(Information.Model.LocationDoorX, Information.Model.LocationDoorY), 
                Information.Model.LocationDoorZ,
                Information.Model.LocationDoorRotation);

            UnitLocker.EnterWriteLock();

            Units.Add(Unit.Id, Unit);

            UnitLocker.ExitWriteLock();

            return Units.ContainsKey(Unit.Id);
        }

        /// <summary>
        /// Gets an Id of the unit from character id.
        /// </summary>
        /// <param name="CharacterId"></param>
        /// <returns></returns>
        public bool GetUnit(int CharacterId, out RoomUnit RoomUnit)
        {
            using (DictionaryAdapter<int, RoomUnit> DA = new DictionaryAdapter<int, RoomUnit>(Units))
            {
                RoomUnit = DA.TryPopValue(GetUnitId(CharacterId));
            }

            return RoomUnit != null;
        }

        /// <summary>
        /// Gets an Id of the unit from character id.
        /// </summary>
        /// <param name="CharacterId"></param>
        /// <returns></returns>
        public int GetUnitId(int CharacterId)
        {
            foreach (RoomUnit Unit in GetUnits(UnitType.Player))
            {
                if (Unit.GetSession().Character.Id == CharacterId)
                {
                    return Unit.Id;
                }
            }

            return -1;
        }

        /// <summary>
        /// Removes an player
        /// </summary>
        /// <param name="Id"></param>
        public bool RemovePlayerByUnitId(int Id)
        {
            UnitLocker.EnterWriteLock();

            if (Units.ContainsKey(Id))
            {
                Units.Remove(Id);
            }

            UnitLocker.ExitWriteLock();

            return !Units.ContainsKey(Id);
        }

        /// <summary>
        /// Removes an player
        /// </summary>
        /// <param name="Id"></param>
        public bool RemovePlayerByCharacterId(int CharacterId)
        {
            return RemovePlayerByUnitId(GetUnitId(CharacterId));
        }

        /// <summary>
        /// Broadcast of packets.
        /// </summary>
        /// <param name="Composer"></param>
        public void WriteComposer(PacketComposer Composer)
        {
            foreach (RoomUnit Unit in GetUnits(UnitType.Player))
            {
                Unit.GetSession().WriteComposer(Composer);
            }
        }

        /// <summary>
        /// Check if need to handle the units.
        /// </summary>
        /// <param name="Units"></param>
        /// <returns></returns>
        public bool GetUnitUpdates(out ICollection<RoomUnit> Units)
        {
            Units = new List<RoomUnit>();

            foreach (RoomUnit Unit in this.Units.Values)
            {
                if (Unit.ProgressNodes.Count > 0 || Unit.CurrentNode != null || Unit.NeedsUpdate)
                {
                    Units.Add(Unit);
                }
            }

            return Units.Count > 0;
        }

        /// <summary>
        /// Checks if need to handle the kick users.
        /// </summary>
        /// <param name="Units"></param>
        /// <returns></returns>
        public bool GetUnitsToKick(out ICollection<RoomUnit> Units) // TODO LET THEM SLEEP AT 0.5 #TIME
        {
            Units = new List<RoomUnit>();

            foreach (RoomUnit Unit in this.Units.Values)
            {
                if ((DateTime.Now - Unit.LastUpdate).TotalMinutes >= System.Configuration.PopInt32("Rooms.Inactivity.Unit.Kick.Minutes")
                    || (Unit.Point == new Point(Information.Model.LocationDoorX,Information.Model.LocationDoorY) 
                    && Unit.TilesWalked > 0) || Unit.NeedsRemove)
                {
                    Units.Add(Unit);
                }
            }

            return Units.Count > 0;
        }

        /// <summary>
        /// Closes all resources.
        /// </summary>
        public void Dispose() { }
    }
}
