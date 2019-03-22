using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Linq;

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
        public static IEnumerable<T> GetList<T>(this IDbConnection connection, string strWhere = "", object param = null)
        {
            return connection.Query<T>(FasterCore.GetListSql(typeof(T)) + strWhere, param);
        }
        /// <summary>
        /// 异步加载列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<T>> GetListAsync<T>(this IDbConnection connection, string strWhere = "", object param = null)
        {
            return await connection.QueryAsync<T>(FasterCore.GetListSql(typeof(T)) + strWhere, param);
        }
        /// <summary>
        /// 根据主键单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static T Get<T>(this IDbConnection connection, params object[] keys)
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
        /// 异步根据主键单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="param"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(this IDbConnection connection, params object[] keys)
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
        /// 新增实体列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static int Add<T>(this IDbConnection connection, object param = null)
        {
            return connection.Execute(FasterCore.GetInsertSql(typeof(T)), param);
        }

        /// <summary>
        /// 异步新增实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static async Task<int> AddAsync<T>(this IDbConnection connection, object param = null)
        {
            return await connection.ExecuteAsync(FasterCore.GetInsertSql(typeof(T)), param);
        }
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static int Update<T>(this IDbConnection connection, object param = null)
        {
            return connection.Execute(FasterCore.GetUpdateSql(typeof(T)), param);
        }
        /// <summary>
        /// 异步更新实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static async Task<int> UpdateAsync<T>(this IDbConnection connection, object param = null)
        {
            return await connection.ExecuteAsync(FasterCore.GetUpdateSql(typeof(T)), param);
        }
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static int Remove<T>(this IDbConnection connection, params object[] keys)
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
        public static async Task<int> RemoveAsync<T>(this IDbConnection connection, params object[] keys)
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
        /// <summary>
        /// 分页查询
        /// example
        /// var result=connection.GetPageList<User>("userid,username desc"," where userid>@id",new {id=10},2,20);
        /// int count=result.Item1;
        /// var list=result.Item2;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="order">排序字段</param>
        /// <param name="strWhere"></param>
        /// <param name="param"></param>
        /// <param name="pageNum"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static Tuple<int, IEnumerable<T>> GetPageList<T>(this IDbConnection connection, string order, string strWhere = "", object param = null, int pageNum = 1, int PageSize = 10)
        {
            int count = connection.GetValue<int>(FasterCore.GetCountSql(typeof(T)) + strWhere, param);
            var query = connection.Query<T>(FasterCore.GetPageListSql(typeof(T), order, strWhere, pageNum, PageSize), param);
            return Tuple.Create(count, query);
        }

        /// <summary>
        /// 查询单个结果
        /// </summary>
        /// <typeparam name="T">int/string</typeparam>
        /// <param name="strSql">sql语句</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static T GetValue<T>(this IDbConnection connection, string strSql, object param = null)
        {
            return connection.ExecuteScalar<T>(strSql, param);
        }

        /// <summary>
        /// 执行多条语句事务
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sqlAndParams"></param>
        /// <returns></returns>
        public static bool ExecuteTrans(this IDbConnection connection, Dictionary<string, object> sqlAndParams)
        {
            connection.Open();
            var transaction = connection.BeginTransaction();
            try
            {
                foreach (var item in sqlAndParams)
                    connection.Execute(item.Key, item.Value, transaction);

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                //CommonFun.WriteLog(ex) 记录日志
                transaction.Rollback();
                return false;
            }
        }

        /// <summary>
        /// 执行存储过程加载数据
        /// example :
        /// var p = new DynamicParameters();
        /// p.Add("@a", 11);
        /// p.Add("@b", dbType: DbType.Int32, direction: ParameterDirection.Output);
        /// p.Add("@c", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        /// cnn.Execute("spMagicProc", p, commandType: CommandType.StoredProcedure);        
        /// int b = p.Get<int>("@b");
        /// int c = p.Get<int>("@c");
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="storeProcedure">存储过程名称</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static object ExecuteSP(this IDbConnection connection, string storeProcedure, object param = null)
        {
            connection.Execute(storeProcedure, param, commandType: CommandType.StoredProcedure);
            return param;
        }

        /// <summary>
        /// 根据存储过程加载数据
        /// example
        /// connection.GetListSP<User>("spGetUser", new {Id = 1})
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="storeProcedure">存储过程名称</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static IEnumerable<T> GetListSP<T>(this IDbConnection connection, string storeProcedure, object param = null)
        {
            return connection.Query<T>(storeProcedure, param, commandType: CommandType.StoredProcedure);
        }
    }



}
