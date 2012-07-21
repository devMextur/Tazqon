using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Tazqon.Storage
{
    class MySQLManager
    {
        /// <summary>
        /// String to activate Streams with.
        /// </summary>
        public string QueryHandlerString { get; private set; }

        public MySQLManager()
        {
            MySqlConnectionStringBuilder sb = new MySqlConnectionStringBuilder();
            sb.Server = System.Configuration.PopString("MySQL.Connection.Host");
            sb.Port = System.Configuration.PopUInt32("MySQL.Connection.Port");
            sb.UserID = System.Configuration.PopString("MySQL.Information.Username");
            sb.Password = System.Configuration.PopString("MySQL.Information.Password");
            sb.Database = System.Configuration.PopString("MySQL.Information.Database");
            sb.Pooling = System.Configuration.PopBoolean("MySQL.Pooling.Enabled");
            sb.MinimumPoolSize = System.Configuration.PopUInt32("MySQL.Pooling.Minimal");
            sb.MaximumPoolSize = System.Configuration.PopUInt32("MySQL.Pooling.Maximal");

            this.QueryHandlerString = sb.ConnectionString;
        }

        /// <summary>
        /// Invokes the query, there is no output.
        /// </summary>
        /// <param name="Query"></param>
        public void InvokeQuery(Query Query)
        {
            GetObject(Query);
        }

        /// <summary>
        /// Returns an quick Obj.
        /// </summary>
        /// <param name="Query"></param>
        /// <returns></returns>
        public QueryObject GetObject(Query Query)
        {
            using (var Stream = new QueryStream())
            {
                Stream.Push(Query);
                return Stream.Pop();
            }
        }

        /// <summary>
        /// Returns an quick stack.
        /// </summary>
        /// <param name="Querys"></param>
        /// <returns></returns>
        public Stack<QueryObject> GetObjects(params Query[] Querys)
        {
            Stack<QueryObject> Stack = new Stack<QueryObject>();

            using (var Stream = new QueryStream())
            {
                foreach (var Query in Querys)
                {
                    Stream.Push(Query);
                }

                for (int i = 0; i <= Stream.Querys.Count; i++)
                {
                    Stack.Push(Stream.Pop());
                }
            }

            return Stack;
        }
    }
}
