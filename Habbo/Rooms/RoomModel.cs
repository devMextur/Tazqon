using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Tazqon.Commons.Adapters;
using Tazqon.Habbo.Characters;

namespace Tazqon.Habbo.Rooms
{
    class RoomModel
    {
        /// <summary>
        /// Id of the model.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Caption of the model.
        /// </summary>
        public string Caption { get; private set; }

        /// <summary>
        /// Map of the model.
        /// </summary>
        public string Map { get; private set; }

        /// <summary>
        /// Location x of the model door.
        /// </summary>
        public int LocationDoorX { get; private set; }

        /// <summary>
        /// Location y of the model door.
        /// </summary>
        public int LocationDoorY { get; private set; }

        /// <summary>
        /// Location z of the model door.
        /// </summary>
        public double LocationDoorZ { get; private set; }

        /// <summary>
        /// Location rotation of the model door.
        /// </summary>
        public int LocationDoorRotation { get; private set; }

        /// <summary>
        /// Maxmial units of the model.
        /// </summary>
        public int MaximalUnits { get; private set; }

        /// <summary>
        /// Required rank of the model.
        /// </summary>
        public Membership RequiredMembership { get; private set; }

        /// <summary>
        /// Nodes generated from the map.
        /// </summary>
        public ICollection<TileNode> Nodes { get; private set; }

        /// <summary>
        /// Parameters With Door;
        /// </summary>
        public string ParametersWithDoor { get; private set; }

        /// <summary>
        /// Parameters With Out Door;
        /// </summary>
        public string ParametersWithOutDoor { get; private set; }

        public RoomModel(DataRow Row)
        {
            using (RowAdapter Adapter = new RowAdapter(Row))
            {
                this.Id = Adapter.PopInt32("id");
                this.Caption = Adapter.PopString("caption");
                this.Map = Adapter.PopString("map").Replace(((char)10).ToString(),string.Empty);
                this.LocationDoorX = Adapter.PopInt32("location_door_x");
                this.LocationDoorY = Adapter.PopInt32("location_door_y");
                this.LocationDoorZ = Adapter.PopInt32("location_door_z");
                this.LocationDoorRotation = Adapter.PopInt32("location_door_rotation");
                this.MaximalUnits = Adapter.PopInt32("maximal_units");
                this.RequiredMembership = Adapter.PopEnum<Membership>("required_membership");
            }

            this.Nodes = new List<TileNode>();

            for (int y = 0; y < this.Map.Split((char)13).Length; y++)
            {
                var Line = this.Map.Split((char)13)[y];

                for (int x = 0; x < Line.Length; x++)
                {
                    char Item = Line[x];

                    TileNode Node = new TileNode(Item, x, y);

                    Nodes.Add(Node);
                }
            }

            this.ParametersWithDoor = GetParametersWithDoor();
            this.ParametersWithOutDoor = GetParametersWithOutDoor();
        }

        /// <summary>
        /// Generates paremeters for the client with door.
        /// </summary>
        /// <returns></returns>
        public string GetParametersWithDoor()
        {
            string Builder = string.Empty;

            foreach (TileNode Node in Nodes)
            {
                if (!string.IsNullOrEmpty(Builder) && Node.IX == 0)
                {
                    Builder += ((char)13).ToString();
                }

                if (Node.IX == LocationDoorX && Node.IY == LocationDoorY)
                {
                    Builder += (int)LocationDoorZ;
                    continue;
                }

                if (Node.DeadTile)
                {
                    Builder += 'x';
                }
                else Builder += (int)Node.Height;
            }

            return Builder + ((char)13).ToString();
        }

        /// <summary>
        /// Generates paremeters for the client with out door.
        /// </summary>
        /// <returns></returns>
        public string GetParametersWithOutDoor()
        {
            string Builder = string.Empty;

            foreach (TileNode Node in Nodes)
            {
                if (!string.IsNullOrEmpty(Builder) && Node.IX == 0)
                {
                    Builder += ((char)13).ToString();
                }

                if (Node.DeadTile)
                {
                    Builder += 'x';
                }
                else Builder += (int)Node.Height;
            }

            return Builder + ((char)13).ToString();
        }
    }

    class TileNode
    {
        public const char BLACK_TILE = 'x';

        public bool DeadTile { get; private set; }

        public int IX { get; private set; }
        public int IY { get; private set; }

        public double Height { get; private set; }

        public int K
        {
            get { return IX * IY; }
        }
        
        public TileNode(char Information, int X, int Y)
        {
            this.Height = -1;
            this.DeadTile = (Information == BLACK_TILE);

            if (!DeadTile)
            {
                double zHeight = 0.0;

                if (double.TryParse(Information.ToString(), out zHeight))
                {
                    this.Height = zHeight;
                }
            }

            this.IX = X;
            this.IY = Y;
        }
    }
}
