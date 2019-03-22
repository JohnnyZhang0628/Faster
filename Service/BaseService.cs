using Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Faster;

namespace Service
{
    public class BaseService : IRepository
    {
        private IDbConnection _dbConnection { set; get; }

        private const string _connectionStr = "server=.;database=test;user id=sa;password=55969126";

        public BaseService()
        {
            _dbConnection = new SqlConnection(_connectionStr);
        }

        public int Add<T>(IEnumerable<T> modelList)
        {
            return _dbConnection.Add<T>(modelList);
        }

        public T Get<T>(params object[] param)
        {
            return _dbConnection.Get<T>(param);
        }

        public IEnumerable<T> GetList<T>(string strWhere = "",object param=null)
        {
            return _dbConnection.GetList<T>(strWhere,param);
        }

        public int Update<T>(IEnumerable<T> modelList)
        {
            return _dbConnection.Update<T>(modelList);
        }

        public int Remove<T>(params object[] param)
        {

            return _dbConnection.Remove<T>(param);
        }

        public Tuple<int, IEnumerable<T>> GetPageList<T>(string order, string strWhere = "", object param = null, int pageNum = 1, int PageSize = 10)
        {
            return _dbConnection.GetPageList<T>(order, strWhere, param, pageNum, PageSize);
        }
    }
}
