using System;
using System.Collections.Generic;
using Faster;
using System.Data;

namespace Repository
{
    public interface IRepository
    {
        IEnumerable<T> GetList<T>(string strWhere = "", object param = null) where T : new();
        T Get<T>( object param = null) where T : new();

        int Add<T>(T model);
        int BulkAdd<T>(IEnumerable<T> modelList);

        int Update<T>(T model);
        int BulkUpdate<T>(IEnumerable<T> modelList);

        int Remove<T>(object param = null);

        Tuple<int, IEnumerable<T>> GetPageList<T>(string order, string strWhere = "", object param = null, int pageNum = 1, int PageSize = 10) where T : new();

    }
}
