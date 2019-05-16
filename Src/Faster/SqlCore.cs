using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
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
        public static IEnumerable<T> ExecuteQuery<T>(this IDbConnection connection, string strSql, object param = null)
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
        /// 批量新增实体,生成一条语句。优化速度
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static int BulkInsert<T>(this IDbConnection connection, IEnumerable<T> param)
        {
            StringBuilder strSql = new StringBuilder();
            return connection.Execute(command =>
            {
                //生成语句
                var myTable = FasterCore<T>.GetPropTable();
                var query = myTable.Columns.Where(m => m.Identity == false);

                int i = 0;
                foreach (var item in param)
                {
                    strSql.Append($"insert into {myTable.Name} ({string.Join(",", query.Select(m => $"{m.Alias}"))}) values");
                    strSql.Append($" ({string.Join(",", query.Select(m => $"@{m.Name + i}"))});");
                    i++;
                }

                command.CommandText = strSql.ToString();
                command.AddParams(param);

                return command.ExecuteNonQuery();
            });

        }


        /// <summary>
        /// 批量删除实体,生成一条语句。优化速度
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static int BulkRemove<T>(this IDbConnection connection, IEnumerable<T> param)
        {
            StringBuilder strSql = new StringBuilder();
            return connection.Execute(command =>
            {
                //生成语句
                var myTable = FasterCore<T>.GetPropTable();
                var query = myTable.Columns.Where(m => m.Identity == false);

                int i = 0;
                foreach (var item in param)
                {
                    strSql.Append($"delete from  {myTable.Name}  where {string.Join(" and ", query.Select(m => $"{m.Alias}=@{m.Name + i}"))} ;");
                    i++;
                }

                command.CommandText = strSql.ToString();
                command.AddParams(param);

                return command.ExecuteNonQuery();
            });

        }




        /// <summary>
        /// 执行查询语句，返回动态类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="strSql">sql</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static IEnumerable<dynamic> ExecuteQueryDynamic(this IDbConnection connection, string strSql, object param = null)
        {
            return connection.Execute(command =>
            {
                command.CommandText = strSql;
                command.AddParams(param);
                using (var reader = command.ExecuteReader())
                {
                    return reader.ReaderToDynamic();
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

                    command.AddParams(param);
                    count = command.ExecuteNonQuery();

                }
                else
                    count = command.ExecuteNonQuery();
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
        public static IEnumerable<T> ExecuteQuerySP<T>(this IDbConnection connection, string storeProcedure, IDbDataParameter[] parameters = null)
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
        private static IEnumerable<T> ReaderToEntity<T>(this IDataReader reader)
        {


            List<T> list = new List<T>();
            var myTable = FasterCore<T>.GetPropTable();
            while (reader.Read())
            {
                T t = Activator.CreateInstance<T>();
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
        /// IDataReader转化为动态类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static IEnumerable<dynamic> ReaderToDynamic(this IDataReader reader)
        {

            List<dynamic> list = new List<dynamic>();

            while (reader.Read())
            {
                dynamic dynamicObj = new ExpandoObject();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    ((IDictionary<string, object>)dynamicObj).Add(reader.GetName(i), reader.GetValue(i) == DBNull.Value ? null : reader.GetValue(i));
                }
                list.Add(dynamicObj);
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
                    dbParam.Value = prop.GetValue(param) == null ? DBNull.Value : prop.GetValue(param);
                    command.Parameters.Add(dbParam);
                }
            }
        }

        /// <summary>
        /// 批量新增时，将所有参数添加
        /// </summary>
        /// <param name="command"></param>
        /// <param name="param"></param>
        private static void AddParams<T>(this IDbCommand command, IEnumerable<T> param)
        {
            command.Parameters.Clear();
            if (param != null)
            {
                int i = 0;
                foreach (var item in param)
                {
                    foreach (var prop in item.GetType().GetProperties())
                    {
                        var dbParam = command.CreateParameter();
                        dbParam.ParameterName = $"@{prop.Name + i}";
                        dbParam.Value = prop.GetValue(item) == null ? DBNull.Value : prop.GetValue(item);
                        command.Parameters.Add(dbParam);

                    }
                    i++;
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
