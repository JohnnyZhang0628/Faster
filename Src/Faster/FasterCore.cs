using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Faster
{
    public static class FasterCore<T>
    {
        //缓存类
        private static Table myTable = new Table();


        public static Table GetPropTable()
        {
            return myTable;
        }
        static FasterCore()
        {
            var type = typeof(T);
            myTable.Name = type.Name;
            //获取表名
            var classAttribute = type.GetCustomAttribute(typeof(FasterTableAttribute)) as FasterTableAttribute;
            if (classAttribute != null && classAttribute.TableName != null)
                myTable.Name = (classAttribute as FasterTableAttribute).TableName;

            //获取列
            foreach (var item in type.GetProperties())
            {
                var column = new Column();
                column.Name = item.Name;
                column.Alias = item.Name;
                //别名
                var colAttribute = item.GetCustomAttribute(typeof(FasterColumnAttribute)) as FasterColumnAttribute;
                if (colAttribute != null && colAttribute.ColumnName != null)
                    column.Alias = (colAttribute as FasterColumnAttribute).ColumnName;
                //主键
                var keyAttribute = item.GetCustomAttribute(typeof(FasterKeyAttribute)) as FasterKeyAttribute;
                if (keyAttribute != null && keyAttribute.Key)
                    column.Key = true;
                //自增长ID
                var identityAttribute = item.GetCustomAttribute(typeof(FasterIdentityAttribute)) as FasterIdentityAttribute;
                if (identityAttribute != null && identityAttribute.Identity)
                    column.Identity = true;
                column.Type = item.PropertyType.Name;
                myTable.Columns.Add(column);
            }

            //判断有没有主键、
            if (myTable.Columns.Where(m => m.Key == true).Count() == 0)
                throw new Exception($"{type.Name} class must have a key");

        }



        public static string GetListSql()
        {
            return $"select {string.Join(",", myTable.Columns.Select(m => m.Alias))} from {myTable.Name} ";
        }

        public static string GetSql()
        {
            return $"select {string.Join(",", myTable.Columns.Select(m => m.Alias))} from {myTable.Name} " +
                $"where {string.Join(" and ", myTable.Columns.Where(m => m.Key == true).Select(m => $"{m.Alias}=@{m.Name}"))}";
        }

        public static string GetInsertSql()
        {
            //去掉自增长ID
            var query = myTable.Columns.Where(m => m.Identity == false);

            return $"insert into {myTable.Name} ({string.Join(",", query.Select(m => $"{m.Alias}"))}) " +
          $"values({string.Join(",", query.Select(m => $"@{m.Name}"))})";
        }

        public static string GetUpdateSql()
        {
            return $" update {myTable.Name} set {string.Join(",", myTable.Columns.Where(m => m.Key != true).Select(m => $"{m.Alias}=@{m.Name}"))} " +
                $"where {string.Join(" and ", myTable.Columns.Where(m => m.Key == true).Select(m => $"{m.Alias}=@{m.Name}"))}";
        }

        public static string GetDeleteSql()
        {
            return $" delete {myTable.Name} where {string.Join(" and ", myTable.Columns.Where(m => m.Key == true).Select(m => $"{m.Alias}=@{m.Name}"))}";
        }

        public static string GetCountSql()
        {

            return $" select count(*) from {myTable.Name} ";
        }

        public static string GetPageListSql(string order, string strWhere = "", int pageNum = 1, int PageSize = 10)
        {
            string strColumns = string.Join(",", myTable.Columns.Select(m => m.Alias));
            StringBuilder strSql = new StringBuilder();
            strSql.Append($" select {strColumns} from ");
            strSql.Append($"( select row_number() over(order by {order}) as pageNum,{strColumns} from {myTable.Name}  {strWhere} ) t");
            strSql.Append($" where pageNum between {(pageNum - 1) * PageSize+1} and {pageNum * PageSize} ");

            return strSql.ToString();
        }


    }

}
