using System;

namespace Faster
{
    public class SqlTypeMap
    {
        /// <summary>
        /// sql server 类型转化为C# 类型
        /// </summary>
        /// <param name="sqlType"></param>
        /// <returns></returns>
        public static string SqlType2CsharpType(string sqlType)
        {
            switch (sqlType)
            {
                case "bigint":
                    return "long";
                case "binary":
                    return "object";
                case "bit":
                    return "bool";
                case "char":
                    return "string";
                case "datetime":
                    return "DateTime";
                case "date":
                    return "DateTime";
                case "decimal":
                    return "decimal";
                case "float":
                    return "double";
                case "image":
                    return "object";
                case "int":
                    return "int";
                case "money":
                    return "decimal";
                case "nchar":
                    return "string";
                case "next":
                    return "string";
                case "nvarchar":
                    return "string";
                case "real":
                    return "float";
                case "smalldatetime":
                    return "DateTime";
                case "smallint":
                    return "short";
                case "smallmoney":
                    return "decimal";
                case "text":
                    return "string";
                case "timestamp":
                    return "object";
                case "tinyint":
                    return "byte";
                case "uniqueidentifier":
                    return "object";
                case "varbinary":
                    return "object";
                case "varchar":
                    return "string";
                case "variant":
                    return "object";
                case "xml":
                    return "object";
                default:
                    {
                        throw new Exception($" sql server {sqlType} type can't map to C# type!");
                    }
            }
        }
        /// <summary>
        /// c# 类型转化为Sql server类型
        /// </summary>
        /// <param name="csharpType"></param>
        /// <returns></returns>
        public static string CsharpType2SqlType(string csharpType)
        {
            switch (csharpType)
            {
                case "Int64":
                    return "long";
                case "Boolean":
                    return "bit";
                case "String":
                    return "nvarchar(50)";
                case "DateTime":
                    return "datetime";
                case "Decimal":
                    return "decimal";
                case "Double":
                    return "float";
                case "Int32":
                    return "int";
                case "Single":
                    return "real";
                case "Int16":
                    return "smallint";
                case "Byte":
                    return "tinyint";
                default:
                    {
                        throw new Exception($"C# {csharpType} type can't map to Sql Server type!");
                    };
            }
        }
    }
}
