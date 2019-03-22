using System;
using System.Collections.Generic;
using Faster;
using System.Data;

namespace Repository
{
    public interface IRepository
    {
        IEnumerable<T> GetList<T>(string strWhere = "",object param=null);
        T Get<T>(params object[] param);

        int Add<T>(IEnumerable<T> modelList);

        int Update<T>(IEnumerable<T> modelList);
        int Remove<T>(params object[] param);
        Tuple<int, IEnumerable<T>> GetPageList<T>(string order, string strWhere = "", object param = null, int pageNum = 1, int PageSize = 10);

    }
}
