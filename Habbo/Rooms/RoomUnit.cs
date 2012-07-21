using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Tazqon.Habbo.Characters;
using Tazqon.Habbo.Pathfinding;
using Tazqon.Network;

namespace Tazqon.Habbo.Rooms
{
    class RoomUnit
    {
        /// <summary>
        /// Id of the unit.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// What kind of unit is this unit?
        /// </summary>
        public UnitType Type { get; private set; }

        /// <summary>
        /// Is the user authenticated and deployed?
        /// </summary>
        public bool Authenticated { get; private set; }

        /// <summary>
        /// Last action of the unit.
        /// </summary>
        public DateTime LastUpdate { get; private set; }

        /// <summary>
        /// Id of the base.
        /// </summary>
        public int BaseId { get; private set; }

        /// <summary>
        /// Current X location.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// Current Y location.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// Current Z location
        /// </summary>
        public double Z { get; private set; }

        /// <summary>
        /// Current Rotation.
        /// </summary>
        public int Rotation { get; private set; }

        /// <summary>
        /// Current location
        /// </summary>
        public Point Point
        {
            get { return new Point(X, Y); }
        }

        /// <summary>
        /// Amount of tiles walken.
        /// </summary>
        public int TilesWalked { get; private set; }

        /// <summary>
        /// Activities of this unit.
        /// </summary>
        public Dictionary<string, string> Activities { get; private set; }

        /// <summary>
        /// Stack to walk on.
        /// </summary>
        public Stack<BlockNode> ProgressNodes { get; private set; }

        /// <summary>
        /// CurrentNode of the waling progress.
        /// </summary>
        public BlockNode CurrentNode { get; private set; }

        /// <summary>
        /// If unit need to be updated.
        /// </summary>
        public bool NeedsUpdate { get; private set; }

        /// <summary>
        /// If unit need to be updated.
        /// </summary>
        public bool NeedsRemove { get; private set; }

        public int CurrentDanceId { get; private set; }

        public RoomUnit(int Id, int BaseId, UnitType Type, Point Location, double Z, int Rotation)
        {
            this.Id = Id;
            this.BaseId = BaseId;
            this.Type = Type;
            this.Authenticated = false;
            this.LastUpdate = DateTime.Now;
            this.X = Location.X;
            this.Y = Location.Y;
            this.Z = Z;
            this.Rotation = Rotation;
            this.TilesWalked = 0;
            this.Activities = new Dictionary<string, string>();
            this.ProgressNodes = new Stack<BlockNode>();
            this.NeedsUpdate = false;
            this.NeedsRemove = false;
            this.CurrentDanceId = 0;
        }

        /// <summary>
        /// Authenticates the unit.
        /// </summary>
        public void Authenticate()
        {
            this.Authenticated = true;
        }

        public void Remove()
        {
            this.NeedsRemove = true;
        }

        /// <summary>
        /// Walks one tile to the next item.
        /// </summary>
        public bool WalkOneTile()
        {
            if (ProgressNodes.Count <= 0 && CurrentNode == null)
            {
                return false;
            }

            if (CurrentNode != null)
            {
                this.X = CurrentNode.Point.X;
                this.Y = CurrentNode.Point.Y;
                this.CurrentNode = null;
                this.TilesWalked++;
            }

            if (ProgressNodes.Count > 0)
            {
                var Node = ProgressNodes.Pop();

                DoActivity("mv", string.Format("{0},{1},{2}", Node.Point.X, Node.Point.Y, "0.0"));

                this.CurrentNode = Node;
                this.Rotation = System.HabboSystem.RoomManager.BlockCalculator.GetRotation(this.Point, this.CurrentNode.Point);
            }
            else
            {
                Activities.Remove("mv");
            }

            return true;
        }

        public void SetLocation(Point Location)
        {
            this.X = Location.X;
            this.Y = Location.Y;
        }

        public void StopWalk()
        {
            lock (this)
            {
                this.ProgressNodes.Clear();
                this.WalkOneTile();
            }
        }

        public void LookAt(Point Point)
        {
            this.Rotation = System.HabboSystem.RoomManager.BlockCalculator.GetRotation(this.Point, Point);
            this.NeedsUpdate = true;
        }

        public void GivePath(Stack<BlockNode> Nodes)
        {
            this.ProgressNodes = Nodes;
        }

        public void DoActivity(string Key, string Value)
        {
            if (Activities.ContainsKey(Key))
            {
                Activities[Key] = Value;
            }
            else Activities.Add(Key, Value);
        }

        public void Dance(int Obj)
        {
            this.CurrentDanceId = Obj;
        }

        public void Activate()
        {
            this.LastUpdate = DateTime.Now;
        }

        public Session GetSession()
        {
            return System.NetworkSocket.GetSession(BaseId);
        }
    }

    enum UnitType
    {
        Player,
        Robot
    }
}
