using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Faster
{
    public static class SqlCore
    {
        /// <summary>
        /// 执行查询语句命令
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="strSql">sql</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static IEnumerable<T> ExecuteQuery<T>(this IDbConnection connection, string strSql, object param = null) where T : new()
        {
            return connection.Execute(command =>
            {
                command.CommandText = strSql;
                command.AddParams(param);
                using (var reader = command.ExecuteReader())
                {
                    return reader.ReaderToEntity<T>();
                }
            });

        }


        /// <summary>
        /// 执行查询语句命令(多个数据集)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="strSql">sql</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> ExecuteQueryMultiple<T>(this IDbConnection connection, string strSql, object param = null) where T : new()
        {
            return connection.Execute(command =>
            {
                command.CommandText = strSql;
                command.AddParams(param);
                using (var reader = command.ExecuteReader())
                {
                    var multipleList = new List<IEnumerable<T>>();
                    multipleList.Add(reader.ReaderToEntity<T>());
                    while (reader.NextResult())
                    {
                        multipleList.Add(reader.ReaderToEntity<T>());
                    }
                    return multipleList;
                }
            });

        }

        /// <summary>
        /// 执行修改命令语句
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="strSql">sql</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(this IDbConnection connection, string strSql, object param = null)
        {
            return connection.Execute(command =>
            {
                command.CommandText = strSql;
                int count = 0;
                if (param != null)
                {
                    if (param is IEnumerable)
                    {
                        foreach (var item in param as IEnumerable)
                        {
                            command.AddParams(item);
                            count += command.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        command.AddParams(param);
                        count = command.ExecuteNonQuery();
                    }
                }
                return count;
            });
        }

        /// <summary>
        /// 获取第一行第一列的结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="strSql">sql</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static T ExecuteScalar<T>(this IDbConnection connection, string strSql, object param = null)
        {
            return connection.Execute(command =>
           {
               command.CommandText = strSql;
               command.AddParams(param);
               return (T)command.ExecuteScalar();
           });
        }
        /// <summary>
        /// 执行无返回值的存储过程
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="storeProcedure">存储过程名称</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public static int ExecuteNonQuerySP(this IDbConnection connection, string storeProcedure, IDbDataParameter[] parameters = null)
        {
            return connection.Execute(command =>
            {
                command.CommandText = storeProcedure;
                command.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    foreach (var item in parameters)
                    {
                        command.Parameters.Add(item);
                    }
                }
                return command.ExecuteNonQuery();
            });
        }
        /// <summary>
        /// 执行查询存储过程
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="storeProcedure">存储过程名称</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public static IEnumerable<T> ExecuteQuerySP<T>(this IDbConnection connection, string storeProcedure, IDbDataParameter[] parameters = null) where T : new()
        {
            return connection.Execute(command =>
            {
                command.CommandText = storeProcedure;
                command.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                {
                    foreach (var item in parameters)
                    {
                        command.Parameters.Add(item);
                    }
                }

                using (var reader = command.ExecuteReader())
                {
                    return reader.ReaderToEntity<T>();
                }

            });
        }

        /// <summary>
        /// IDataReader转化为实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static IEnumerable<T> ReaderToEntity<T>(this IDataReader reader) where T : new()
        {
            List<T> list = new List<T>();
            var myTable = FasterCore<T>.GetPropTable();
            while (reader.Read())
            {
                T t = new T();
                foreach (var item in myTable.Columns)
                {
                    if (reader.Contain(item.Alias))
                    {
                        var propInfo = typeof(T).GetProperty(item.Name);
                        propInfo.SetValue(t, reader[item.Alias] == DBNull.Value ? null : reader[item.Alias]);
                    }

                };
                list.Add(t);
            }
            return list;
        }
        /// <summary>
        /// 判断IDataReader是否包含某个字段
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        private static bool Contain(this IDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName))
                {
                    return true;
                };
            }
            return false;
        }

        /// <summary>
        /// 对象转化为参数
        /// </summary>
        /// <param name="command"></param>
        /// <param name="param"></param>
        private static void AddParams(this IDbCommand command, object param)
        {
            command.Parameters.Clear();
            if (param != null)
            {
                foreach (var prop in param.GetType().GetProperties())
                {
                    var dbParam = command.CreateParameter();
                    dbParam.ParameterName = $"@{prop.Name}";
                    dbParam.Value = prop.GetValue(param) == DBNull.Value ? null : prop.GetValue(param);
                    command.Parameters.Add(dbParam);
                }
            }


        }
        /// <summary>
        /// 数据库连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        private static T Execute<T>(this IDbConnection connection, Func<IDbCommand, T> func)
        {
            IDbTransaction trans = null;
            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                trans = connection.BeginTransaction();
                IDbCommand command = connection.CreateCommand();
                command.Transaction = trans;
                var result = func.Invoke(command);
                trans.Commit();
                connection.Close();
                return result;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                connection.Close();
                throw ex;
            }

        }



    }
}
