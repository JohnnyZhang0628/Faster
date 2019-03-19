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

        public static string GetListSql(Type type)
        {
            strSql.Clear();
            string tableName = type.Name;
            //获取表名
            var classAttribute = type.GetCustomAttribute(typeof(FasterMappingAttribute)) as FasterMappingAttribute;
            if (classAttribute != null&&classAttribute.TableName!=null)
                tableName = (classAttribute as FasterMappingAttribute).TableName;

            strSql.Append($" select * from [{tableName}]");
            return strSql.ToString();
        }

        public static string GetSql(Type type)
        {
            strSql.Clear();
            string tableName = type.Name;
            //获取表名
            var classAttribute = type.GetCustomAttribute(typeof(FasterMappingAttribute)) as FasterMappingAttribute;
            if (classAttribute != null && classAttribute.TableName != null)
                tableName = (classAttribute as FasterMappingAttribute).TableName;

            strSql.Append($" select * from [{tableName}]");
            foreach (var item in type.GetProperties())
            {
                string columnName = item.Name;
                var propAttribute = item.GetCustomAttribute(typeof(FasterMappingAttribute)) as FasterMappingAttribute;
                if (propAttribute != null&&propAttribute.ColumnName!=null)
                    columnName = (propAttribute as FasterMappingAttribute).ColumnName;

            }

            strSql.Append(string.Join(",", type.GetProperties().Select(p => $"[{ p.Name}]")));
            strSql.Append($" from [{tableName}]");
            return "";
        }

        public static string GetInsertSql(Type type)
        {
            strSql.Clear();
            strSql.Append($" insert into [{type.Name}] (");
            strSql.Append(string.Join(",", type.GetProperties().Select(p => $"[{p.Name}]")));
            strSql.Append($")values( {string.Join(",", type.GetProperties().Select(p => $"@{p.Name}"))});");
            return strSql.ToString();
        }

        public static string GetUpdateSql(Type type)
        {
            strSql.Clear();
            strSql.Append($" update [{type.Name}] set ");
            strSql.Append(string.Join(",", type.GetProperties().Select(p => $"[{p.Name}]=@{p.Name}")));
            return strSql.ToString();
        }





    }
}
