namespace Tazqon.Storage.Querys
{
    class AuthenticateQuery : Query
    {
        public AuthenticateQuery(string Ticket)
        {
            base.Listen("SELECT * FROM characters WHERE sso_ticket = @ticket LIMIT 1", QueryType.DataRow);
            base.Push("ticket", Ticket);
        }
    }

    class CastCharacterQuery : Query
    {
        public CastCharacterQuery(int CharacterId)
        {
            base.Listen("SELECT * FROM characters WHERE id = @id LIMIT 1", QueryType.DataRow);
            base.Push("id", CharacterId);
        }

        public CastCharacterQuery(string Username)
        {
            base.Listen("SELECT * FROM characters WHERE username = @username LIMIT 1", QueryType.DataRow);
            base.Push("username", Username);
        }
    }
}
