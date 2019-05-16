using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Model;
using Faster;
using Repository;
using Service;
using System.Linq;
using System.Data;
using FasterContainer;
using System.Data.SqlClient;
using System.Text;

namespace UnitTest
{
    [TestClass]
    public class UnitTestFaster
    {

        IUserRepository user;
        IDbConnection _dbConnection;
        [TestInitialize]
        public void Init()
        {

            // 获取数据库连接
            _dbConnection = BaseService._dbConnection;
            //IOC 测试
            //1、获取容器
            Container container = new Container();
            //2、注册类型
            container.RegisterType<IUserRepository, UserService>();
            //3、创建实例
            user = container.Resolve<IUserRepository>();

        }

        /// <summary>
        /// 测试DB First 和Code First
        /// </summary>
        [TestMethod]
        public void TestMethodDB()
        {
            //DB First
            _dbConnection.CreateModels();
        }

        [TestMethod]
        public void TestMethodCURD()
        {

            //批量新增
            List<User> userList = new List<User>();
            for (int i = 0; i < 10000; i++)
            {
                userList.Add(new User
                {
                    UserName = "张强" + (i + 1),
                    Password = "123456",
                    Email = "237183141@qq.com",
                    Phone = "18516328675"
                });
            }
            user.BulkAdd(userList);

            //批量修改
            userList = new List<User>();
            for (int i = 0; i < 100; i++)
            {
                userList.Add(new User
                {
                    UserId = i + 1,
                    UserName = "张强" + (i + 1),
                    Password = "zq",
                    Email = "zq@qq.com",
                    Phone = "zq"
                });
            }
            user.BulkUpdate(userList);

            //根据主键查询
            var userModel = user.Get<User>(new { UserId = 1, UserName = "张强1" });
            //根据条件查询 
            userList = user.GetList<User>(" where userid>@id", new { id = 10 }).ToList();
            //分页查询
            var result = user.GetPageList<User>("userid ", " where userid>@id", new { id = 10 }, 2, 20);
            // 满足条件总页数
            int count = result.Item1;
            // 第20条，到40条
            IEnumerable<User> list = result.Item2;

            // 根据主键删除
            int delRow = user.Remove<User>(new { UserId = 1, UserName = "张强1" });


        }

        /// <summary>
        /// 测试存储过程查询
        /// </summary>
        [TestMethod]
        public void TestMethodSP()
        {
            // no params
            var query = _dbConnection.ExecuteQuerySP<User>("sp_test_no_params");

            // query with params
            IDbDataParameter[] parameters =
            {
                new SqlParameter("@user_id",2)
            };
            query = _dbConnection.ExecuteQuerySP<User>("sp_test", parameters);


            //get out params 
            IDbDataParameter[] outparameters =
            {
                new SqlParameter { ParameterName = "@count",DbType=DbType.Int32, Direction = ParameterDirection.Output }
            };

            _dbConnection.ExecuteNonQuerySP("sp_test_out", outparameters);


            var count = outparameters[0].Value;
        }

        [TestMethod]
        public void TestBulkInsert()
        {
            var query = _dbConnection.GetList<TB_ROLE_BUTTON>("where role_id=1");
            int count = _dbConnection.ExecuteNonQuery("delete tb_role_button where role_id=1");
             count = _dbConnection.Add<TB_ROLE_BUTTON>(query);
        }
    }
}
