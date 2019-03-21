using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.CSharp.RuntimeBinder;

namespace Faster
{
    public static class FasterExecute
    {
        /// <summary>
        /// 加载列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="strWhere"> where id=@id and id>@id </param>
        /// <param name="param"> new {id=3}</param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetList<T>(this IDbConnection connection, string strWhere = "", object param = null, int? commandTimeout = null) where T : class
        {
            return connection.Query<T>(FasterCore.GetListSql(typeof(T)) + strWhere, param, null, true, commandTimeout);
        }
        /// <summary>
        /// 异步加载列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<T>> GetListAsync<T>(this IDbConnection connection, string strWhere = "", object param = null, int? commandTimeout = null) where T : class
        {
            return await connection.QueryAsync<T>(FasterCore.GetListSql(typeof(T)) + strWhere, param, null, commandTimeout);
        }
        /// <summary>
        /// 加载单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static T Get<T>(this IDbConnection connection, params dynamic[] keys) where T : class
        {
            Type type = typeof(T);
            var keyList = FasterCore.Init(type).Columns.Where(m => m.Key == true).Select(m => m.Name).ToList();

            if (keys.Length != keyList.Count())
                throw new Exception($"{type.Name} class has {keyList.Count()} key,but params length is {keys.Length}");

            var param = new DynamicParameters();
            for (int i = 0; i < keyList.Count(); i++)
                param.Add(keyList[i], keys[i]);

            return connection.QueryFirstOrDefault<T>(FasterCore.GetSql(type), param);
        }
        /// <summary>
        /// 异步加载单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(this IDbConnection connection, params dynamic[] keys) where T : class
        {
            Type type = typeof(T);
            var keyList = FasterCore.Init(type).Columns.Where(m => m.Key == true).Select(m => m.Name).ToList();

            if (keys.Length != keyList.Count())
                throw new Exception($"{type.Name} class has {keyList.Count()} key,but params length is {keys.Length}");

            var param = new DynamicParameters();
            for (int i = 0; i < keyList.Count(); i++)
                param.Add(keyList[i], keys[i]);
            return await connection.QueryFirstOrDefaultAsync<T>(FasterCore.GetSql(typeof(T)), param);
        }
        /// <summary>
        /// 新增实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static int Add<T>(this IDbConnection connection, object param = null, int? commandTimeout = null) where T : class
        {
            return connection.Execute(FasterCore.GetInsertSql(typeof(T)), param, null, commandTimeout);
        }

        /// <summary>
        /// 异步新增实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static async Task<int> AddAsync<T>(this IDbConnection connection, object param = null, int? commandTimeout = null) where T : class
        {
            return await connection.ExecuteAsync(FasterCore.GetInsertSql(typeof(T)), param, null, commandTimeout);
        }
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static int Update<T>(this IDbConnection connection, object param = null, int? commandTimeout = null) where T : class
        {
            return connection.Execute(FasterCore.GetUpdateSql(typeof(T)), param, null, commandTimeout);
        }
        /// <summary>
        /// 异步更新实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static async Task<int> UpdateAsync<T>(this IDbConnection connection, object param = null, int? commandTimeout = null) where T : class
        {
            return await connection.ExecuteAsync(FasterCore.GetUpdateSql(typeof(T)), param, null, commandTimeout);
        }
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static int Remove<T>(this IDbConnection connection, params dynamic[] keys) where T : class
        {
            Type type = typeof(T);
            var keyList = FasterCore.Init(type).Columns.Where(m => m.Key == true).Select(m => m.Name).ToList();

            if (keys.Length != keyList.Count())
                throw new Exception($"{type.Name} class has {keyList.Count()} key,but params length is {keys.Length}");

            var param = new DynamicParameters();
            for (int i = 0; i < keyList.Count(); i++)
                param.Add(keyList[i], keys[i]);

            return connection.Execute(FasterCore.GetDeleteSql(type), param);

        }
        /// <summary>
        /// 异步删除实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static async Task<int> RemoveAsync<T>(this IDbConnection connection, params dynamic[] keys) where T : class
        {
            Type type = typeof(T);
            var keyList = FasterCore.Init(type).Columns.Where(m => m.Key == true).Select(m => m.Name).ToList();

            if (keys.Length != keyList.Count())
                throw new Exception($"{type.Name} class has {keyList.Count()} key,but params length is {keys.Length}");

            var param = new DynamicParameters();
            for (int i = 0; i < keyList.Count(); i++)
                param.Add(keyList[i], keys[i]);

            return await connection.ExecuteAsync(FasterCore.GetDeleteSql(type), param);
        }



    }
}
