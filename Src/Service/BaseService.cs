using Repository;
using System;
using System.Collections.Generic;
using System.Data;
using Faster;
using System.Data.SqlClient;
using System.Linq;
namespace Service
{
    public abstract class BaseService : IRepository
    {
        // 静态构造函数实现单例
        public static IDbConnection _dbConnection;

        static BaseService()
        {
            _dbConnection = new SqlConnection("server=.;database=labfiledb2;user id=sa;password=55969126");
        }

        public int Add<T>(T model)
        {
            return _dbConnection.Add<T>(model);
        }

        public int BulkAdd<T>(IEnumerable<T> modelList)
        {
            return _dbConnection.Add<T>(modelList);
        }

        public T Get<T>( object param) where T:new()
        {
            return _dbConnection.Get<T>(param);
        }

        public IEnumerable<T> GetList<T>(string strWhere = "", object param = null) where T : new()
        {
            return _dbConnection.GetList<T>(strWhere, param);
        }

        public int BulkUpdate<T>(IEnumerable<T> modelList)
        {
            return _dbConnection.Update<T>(modelList);
        }

        public int Update<T>(T model)
        {
            return _dbConnection.Update<T>(model);
        }

        public int Remove<T>(object param)
        {

            return _dbConnection.Remove<T>(param);
        }

        public Tuple<int, IEnumerable<T>> GetPageList<T>(string order, string strWhere = "", object param = null, int pageNum = 1, int PageSize = 10) where T:new()
        {
            return _dbConnection.GetPageList<T>(order, strWhere, param, pageNum, PageSize);
        }
    }
}
