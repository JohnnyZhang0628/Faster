using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Faster
{
    public class Core
    {
        StringBuilder strSql = new StringBuilder();
        public IEnumerable<T> GetAll<T>() where T:class
        {
            var type = typeof(T);
            strSql.Clear();
          
            strSql.Append(" select ");
            strSql.Append(string.Join(",", type.GetProperties().Select(p => $"[{p.Name}]")));
            strSql.Append($" from [{type.Name}]");
            return Query<T>(strSql.ToString());
        }



        public T Get<T>(object id) where T : class
        {
            var type = typeof(T);
            strSql.Clear();

            var obj = Activator.CreateInstance(type);

            strSql.Append(" select ");
            strSql.Append(string.Join(",", type.GetProperties().Select(p => $"[{p.Name}]")));
            strSql.Append($" from {type.Name}");
            return Query<T>(strSql.ToString()).FirstOrDefault();
        }

        public bool Create<T>(T model)
        {
            var type = typeof(T);
            strSql.Clear();

          
            strSql.Append($" insert into {type.Name} (");
            strSql.Append(string.Join(",", type.GetProperties().Select(p => $"[{p.Name}]")));
            strSql.Append($")values( {string.Join(",", type.GetProperties().Select(p => $"[{p.GetValue(model)}]"))});");
            return Execute(strSql.ToString());
        }


        public IEnumerable<T> Query<T>(string strSql)
        {
            return null;
        }

        public bool Execute(string strSql)
        {
            return false;
        }


    }
}
