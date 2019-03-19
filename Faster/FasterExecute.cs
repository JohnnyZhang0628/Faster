using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Faster
{
   public static class FasterExecute
    {
        public static IEnumerable<T> GetList<T>(this IDbConnection connection, object param = null, int? commandTimeout = null) where T : class
        {
            return connection.Query<T>(FasterCore.GetListSql(typeof(T)), param, null, true, commandTimeout);
        }

        public static Task<IEnumerable<T>> GetListAsync<T>(this IDbConnection connection, object param = null, int? commandTimeout = null) where T : class
        {
            return connection.QueryAsync<T>(FasterCore.GetListSql(typeof(T)), param, null, commandTimeout);
        }

        public static IEnumerable<T> Get<T>(this IDbConnection connection, params object[] keys) where T : class
        {
            return connection.Query<T>(FasterCore.GetSql(typeof(T)), keys);
        }

        public static Task<IEnumerable<T>> GetAsync<T>(this IDbConnection connection, object param = null, int? commandTimeout = null) where T : class
        {
            return connection.QueryAsync<T>(FasterCore.GetListSql(typeof(T)), param, null, commandTimeout);
        }
    }
}
