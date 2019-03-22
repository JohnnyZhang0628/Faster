using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Faster
{
    public static class FasterCore
    {

        static StringBuilder strSql = new StringBuilder();


        static Dictionary<string, Table> cacheDic = new Dictionary<string, Table>();

        public static Table Init(Type type)
        {
            if (cacheDic.ContainsKey(type.Name))
                return cacheDic[type.Name];
            else
            {
                var myTable = new Table();
                myTable.Name = "[" + type.Name + "]";
                //获取表名
                var classAttribute = type.GetCustomAttribute(typeof(FasterTableAttribute)) as FasterTableAttribute;
                if (classAttribute != null && classAttribute.TableName != null)
                    myTable.Name = "[" + (classAttribute as FasterTableAttribute).TableName + "]";

                //获取列
                foreach (var item in type.GetProperties())
                {
                    var column = new Column();
                    column.Name = item.Name;
                    column.Alias = item.Name;
                    var colAttribute = item.GetCustomAttribute(typeof(FasterColumnAttribute)) as FasterColumnAttribute;
                    if (colAttribute != null && colAttribute.ColumnName != null)
                        column.Alias = (colAttribute as FasterColumnAttribute).ColumnName;
                    var keyAttribute = item.GetCustomAttribute(typeof(FasterKeyAttribute)) as FasterKeyAttribute;
                    if (keyAttribute != null && keyAttribute.Key)
                        column.Key = true;

                    column.Type = item.PropertyType;
                    myTable.Columns.Add(column);
                }

                //判断有没有主键、
                if (myTable.Columns.Where(m => m.Key == true).Count() == 0)
                    throw new Exception($"{type.Name} class must have a key");
                else
                {
                    cacheDic.Add(type.Name, myTable);
                    return myTable;
                }
            }
        }


        public static string GetListSql(Type type)
        {
            var myTable = Init(type);
            return $"select {string.Join(",", myTable.Columns.Select(m => "[" + m.Alias + "]"))} from {myTable.Name} ";
        }



        public static string GetSql(Type type)
        {
            var myTable = Init(type);
            return $"select {string.Join(",", myTable.Columns.Select(m => "[" + m.Alias + "]"))} from {myTable.Name} " +
                $"where {string.Join(" and ", myTable.Columns.Where(m => m.Key == true).Select(m => $"[{m.Alias}]=@{m.Name}"))}";
        }

        public static string GetInsertSql(Type type)
        {
            var myTable = Init(type);
            // 当且仅当主键个数为1且类型为int类型的时候认为是自增长ID
            if (myTable.Columns.Where(m => m.Key == true && m.Type == typeof(int)).Count() == 1)
            {
                var columns = myTable.Columns.Where(m => m.Key != true || m.Type != typeof(int));
                return $"insert into {myTable.Name} ({string.Join(",", columns.Select(m => $"[{m.Alias}]"))}) " +
               $"values({string.Join(",", columns.Select(m => $"@{m.Name}"))})";
            }
            else
                return $"insert into {myTable.Name} ({string.Join(",", myTable.Columns.Select(m => $"[{m.Alias}]"))}) " +
              $"values({string.Join(",", myTable.Columns.Select(m => $"@{m.Name}"))})";
        }

        public static string GetUpdateSql(Type type)
        {
            var myTable = Init(type);
            return $" update {myTable.Name} set {string.Join(",", myTable.Columns.Where(m => m.Key != true).Select(m => $"[{m.Alias}]=@{m.Name}"))} where {string.Join(" and ", myTable.Columns.Where(m => m.Key == true).Select(m => $"[{m.Alias}]=@{m.Name}"))}";
        }

        public static string GetDeleteSql(Type type)
        {
            var myTable = Init(type);
            return $" delete {myTable.Name} where {string.Join(" and ", myTable.Columns.Where(m => m.Key == true).Select(m => $"[{m.Alias}]=@{m.Name}"))}";
        }

        public static string GetCountSql(Type type)
        {
            var myTable = Init(type);
            return $" select count(*) from {myTable.Name} ";
        }

        public static string GetPageListSql(Type type, string order, string strWhere = "", int pageNum = 1, int PageSize = 10)
        {
            var myTable = Init(type);
            string strColumns = string.Join(",", myTable.Columns.Select(m => "[" + m.Alias + "]"));
            StringBuilder strSql = new StringBuilder();
            strSql.Append($" select {strColumns} from ");
            strSql.Append($"( select row_number() over(order by {order}) as pageNum,{strColumns} from {myTable.Name}  {strWhere} ) t");
            strSql.Append($" where pageNum between {(pageNum - 1) * PageSize} and {pageNum * PageSize} ");

            return strSql.ToString();
        }
    }

    public class Table
    {
        /// <summary>
        /// 表名称
        /// </summary>
        public string Name { get; set; }

        public List<Column> Columns { get; set; } = new List<Column>();
    }

    public class Column
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// 列类型
        /// </summary>
        public Type Type { get; set; }
        /// <summary>
        /// 是否为主键
        /// </summary>
        public bool Key { get; set; } = false;
    }
}
