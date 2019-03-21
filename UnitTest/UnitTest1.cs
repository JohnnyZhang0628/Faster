using Microsoft.VisualStudio.TestTools.UnitTesting;
using Faster;
using Model;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        string connectionStr = "server=.;database=test;user id=sa;password=55969126";

        [TestMethod]
        public void TestMethod1()
        {

            using (var connection = new SqlConnection(connectionStr))
            {
                //新增
                for (int i = 0; i < 10000; i++)
                {
                    connection.Add<User>(new User
                    {
                        UserName = "张强" + i,
                        Password = "123456",
                        Email = "237183141@qq.com",
                        Phone = "18516328675"
                    });
                }

                //查询
                var list = connection.GetList<User>();

                var user = connection.Get<User>(1, "张强0");

                //修改
                var updateRow = connection.Update<User>(new User
                {
                    UserId = 1,
                    UserName = "张强0",
                    Password = "zq",
                    Email = "zq@qq.com",
                    Phone = "zq"
                });

                //删除

                var deleteRow = connection.Remove<User>(2, "张强1");
            }
            //var getSql = FasterCore.GetSql(typeof(User));
            //var getListSql = FasterCore.GetListSql(typeof(User));
            //var insertSql = FasterCore.GetInsertSql(typeof(User));
            //var updateSql = FasterCore.GetUpdateSql(typeof(User));
            //var deleteSql = FasterCore.GetDeleteSql(typeof(User));

        }



        [TestInitialize]
        public void Init()
        {

        }
    }
}
