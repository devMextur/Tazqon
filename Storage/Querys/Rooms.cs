namespace Tazqon.Storage.Querys
{
    class RoomModelsQuery : Query
    {
        public RoomModelsQuery()
        {
            base.Listen("SELECT * FROM room_models",QueryType.DataTable);
        }
    }

    class RoomInfoQuery : Query
    {
        public RoomInfoQuery(int RoomId)
        {
            base.Listen("SELECT * FROM private_rooms WHERE id = @id LIMIT 1", QueryType.DataRow);
            base.Push("id", RoomId);
        }
    }

    class RoomsQuery : Query
    {
        public RoomsQuery(int CharacterId)
        {
            base.Listen("SELECT id FROM private_rooms WHERE character_id = @id", QueryType.DataTable);
            base.Push("id", CharacterId);
        }
    }
}
