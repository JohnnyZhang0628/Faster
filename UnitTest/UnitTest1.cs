using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Repository;
using Service;
using System.Collections.Generic;
using Faster;
using System.Data.SqlClient;
using System;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {


        [TestMethod]
        public void TestMethod1()
        {
            var _dbConnection = new SqlConnection("server=.;database=test;user id=sa;password=55969126");

            //Code First
            string modelPath = @"E:\WorkSpace\Faster\Model\bin\Debug\netstandard2.0\Model.dll";
            _dbConnection.CreateTable(modelPath);

            //DB First
            _dbConnection.CreateModels();

            IUserRepository repository = new UserService();

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
            repository.Add(userList);

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
            repository.Update(userList);

            //根据主键查询
            User user = repository.Get<User>(1, "张强1");
            //根据条件查询 
            var query = repository.GetList<User>(" where userid>@id", new { id = 10 });
            //分页查询
            var result = repository.GetPageList<User>("userid ", " where userid>@id", new { id = 10 }, 2, 20);
            // 满足条件总页数
            int count = result.Item1;
            // 第20条，到40条
            IEnumerable<User> list = result.Item2;

            // 根据主键删除
            int delRow = repository.Remove<User>(1, "张强1");


            //用户自定义接口
            repository.Login("zq", "123456");

        }



        [TestInitialize]
        public void Init()
        {

        }
    }
}
