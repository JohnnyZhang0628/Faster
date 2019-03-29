using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data.SqlClient;
using Model;
using Faster;
using FasterContainer;
using Repository;
using Service;
using System.Linq;
using System.Data;

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
            //Code First
            string modelPath = @"D:\WorkSpace\Faster\Src\Model\bin\Debug\netstandard2.0\Model.dll";
            _dbConnection.CreateTable(modelPath);

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
            user.Add(userList);

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
            user.Update(userList);

            //根据主键查询
            var userModel = user.Get<User>(1, "张强1");
            //根据条件查询 
            userList = user.GetList<User>(" where userid>@id", new { id = 10 }).ToList();
            //分页查询
            var result = user.GetPageList<User>("userid ", " where userid>@id", new { id = 10 }, 2, 20);
            // 满足条件总页数
            int count = result.Item1;
            // 第20条，到40条
            IEnumerable<User> list = result.Item2;

            // 根据主键删除
            int delRow = user.Remove<User>(1, "张强1");


            //用户自定义接口
            user.Login("zq", "123456");

        }

        /// <summary>
        /// 测试存储过程查询
        /// </summary>
        [TestMethod]
        public void TestMethodSP()
        {
            var query = _dbConnection.GetListSP<User>("sp_test");
        }
    }
}
