namespace Tazqon.Storage.Querys
{
    class BadgeInformationsQuery : Query
    {
        public BadgeInformationsQuery()
        {
            base.Listen("SELECT * FROM badges_information", QueryType.DataTable);
        }
    }
}
