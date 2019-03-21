using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;

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
                    var colAttribute = item.GetCustomAttribute(typeof(FasterColumnAttribute)) as FasterColumnAttribute;
                    if (colAttribute != null && colAttribute.ColumnName != null)
                        column.Name = (colAttribute as FasterColumnAttribute).ColumnName;
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
            return $"select {string.Join(",", myTable.Columns.Select(m => "[" + m.Name + "]"))} from {myTable.Name} ";
        }



        public static string GetSql(Type type)
        {
            var myTable = Init(type);
            return $"select {string.Join(",", myTable.Columns.Select(m => "[" + m.Name + "]"))} from {myTable.Name} " +
                $"where {string.Join(" and ", myTable.Columns.Where(m => m.Key == true).Select(m => $"[{m.Name}]=@{m.Name}"))}";
        }

        public static string GetInsertSql(Type type)
        {
            var myTable = Init(type);
            // 剔除设置为主键并且类型为int的，因为可能是自增长ID
            var columns = myTable.Columns.Where(m => m.Key != true || m.Type != typeof(int));
            return $"insert into {myTable.Name} ({string.Join(",", columns.Select(p => $"[{p.Name}]"))}) " +
                $"values({string.Join(",", columns.Select(p => $"@{p.Name}"))})";
        }

        public static string GetUpdateSql(Type type)
        {
            var myTable = Init(type);
            return $" update {myTable.Name} set {string.Join(",", myTable.Columns.Where(m => m.Key != true).Select(p => $"[{p.Name}]=@{p.Name}"))} where {string.Join(" and ", myTable.Columns.Where(m => m.Key == true).Select(m => $"[{m.Name}]=@{m.Name}"))}";
        }

        public static string GetDeleteSql(Type type)
        {
            var myTable = Init(type);
            return $" delete {myTable.Name} where {string.Join(" and ", myTable.Columns.Where(m => m.Key == true).Select(m => $"[{m.Name}]=@{m.Name}"))}";
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
        /// 列名称
        /// </summary>
        public string Name { get; set; }
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
