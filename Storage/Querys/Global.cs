namespace Tazqon.Storage.Querys
{
    class MySQLSettingsQuery : Query
    {
        public MySQLSettingsQuery()
        {
            base.Listen("SELECT * FROM server_settings", QueryType.DataTable);
        }
    }
}
