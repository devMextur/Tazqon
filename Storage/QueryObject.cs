using System;
using MySql.Data.MySqlClient;

namespace Tazqon.Storage
{
    class QueryObject
    {
        /// <summary>
        /// Information from query.
        /// </summary>
        public object Output { get; private set; }

        /// <summary>
        /// Pushes query into the void, returns information.
        /// </summary>
        /// <param name="Query"></param>
        public void Push(Query Query)
        {
            try
            {
                switch (Query.OutType)
                {
                    case QueryType.Action:
                        MySqlHelper.ExecuteNonQuery(System.MySQLManager.QueryHandlerString, Query.Command, Query.Parameters.ToArray());
                        break;
                    case QueryType.DataRow:
                        Output = MySqlHelper.ExecuteDataRow(System.MySQLManager.QueryHandlerString, Query.Command, Query.Parameters.ToArray());
                        break;
                    case QueryType.DataTable:
                        Output = MySqlHelper.ExecuteDataset(System.MySQLManager.QueryHandlerString, Query.Command, Query.Parameters.ToArray()).Tables[0];
                        break;
                    case QueryType.String:
                        Output = Convert.ToString(MySqlHelper.ExecuteScalar(System.MySQLManager.QueryHandlerString, Query.Command, Query.Parameters.ToArray()));
                        break;
                    case QueryType.Integer:
                        Output = Convert.ToInt32(MySqlHelper.ExecuteScalar(System.MySQLManager.QueryHandlerString, Query.Command, Query.Parameters.ToArray()));
                        break;
                    case QueryType.Boolean:
                        Output = Convert.ToInt32(MySqlHelper.ExecuteScalar(System.MySQLManager.QueryHandlerString, Query.Command, Query.Parameters.ToArray())) > 0;
                        break;
                }
            }
            catch { Output = null; }

            Query.Dispose();
        }

        /// <summary>
        /// Pushes query into the void, returns information.
        /// </summary>
        /// <param name="Query"></param>
        /// <param name="Output"> </param>
        public static void Push(Query Query, out object Output)
        {
            QueryObject Obj = new QueryObject();
            Obj.Push(Query);
            Output = Obj.Output;

            if (Output == null)
            {
                Output = new object();
            }

            Query.Dispose();
        }

        /// <summary>
        /// Returns an output and disposes it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetOutput<T>()
        {
            return (T)Output;
        }
    }
}
