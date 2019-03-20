using System;
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

        /// <summary>
        /// 获取表名称
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        static string GetTableName(Type type)
        {
            string tableName = type.Name;
            //获取表名
            var classAttribute = type.GetCustomAttribute(typeof(FasterTableAttribute)) as FasterTableAttribute;
            if (classAttribute != null && classAttribute.TableName != null)
                tableName = (classAttribute as FasterTableAttribute).TableName;
            return tableName;
        }
        /// <summary>
        /// 获取列
        /// </summary>
        static IEnumerable<string> GetColumns(Type type)
        {
            List<string> columnList = new List<string>();
            foreach (var item in type.GetProperties())
            {
                string columnName = item.Name;
                var propAttribute = item.GetCustomAttribute(typeof(FasterColumnAttribute)) as FasterColumnAttribute;
                if (propAttribute != null)
                {
                    if (propAttribute.ColumnName != null)
                        columnName = (propAttribute as FasterColumnAttribute).ColumnName;

                }

                columnList.Add("[" + columnName + "]");
            }
            return columnList;
        }
        /// <summary>
        /// 获取主键
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetKeys(Type type)
        {
            List<string> keyList = new List<string>();
            foreach (var item in type.GetProperties())
            {
                var propAttribute = item.GetCustomAttribute(typeof(FasterKeyAttribute)) as FasterKeyAttribute;
                if (propAttribute != null && propAttribute.Key)
                {
                    keyList.Add(item.Name);
                }
            }
            if (keyList.Count == 0)
                throw new Exception($"{type.Name} class must have a key");
            else
                return keyList;
        }

        public static string GetListSql(Type type)
        {
            return $"select {string.Join(",", GetColumns(type))} from [{GetTableName(type)}] ";
        }

       

        public static string GetSql(Type type)
        {
            return $"select {string.Join(",", GetColumns(type))} from [{GetTableName(type)}] where {string.Join(" and ", GetKeys(type).Select(m => $"{m}=@{m}"))}";
        }

        public static string GetInsertSql(Type type)
        {
            var columns = GetColumns(type);
            return $"insert into [{GetTableName(type)}] ({string.Join(",", type.GetProperties().Select(p => $"[{p.Name}]"))}) values({string.Join(",", type.GetProperties().Select(p => $"@{p.Name}"))})";
        }

        public static string GetUpdateSql(Type type)
        {
            return $" update {GetTableName(type)} set {string.Join(",", type.GetProperties().Select(p => $"[{p.Name}]=@{p.Name}"))} where {string.Join(" and ", GetKeys(type).Select(m => $"{m}=@{m}"))}";
        }

        public static string GetDeleteSql(Type type)
        {
            return $" delete {GetTableName(type)} where {string.Join(" and ", GetKeys(type).Select(m => $"{m}=@{m}"))}";
        }
    }
}
