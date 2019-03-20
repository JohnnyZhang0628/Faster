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
        User user = new User
        {
            UserId = 1,
            UserName = "уег©",
            Password = "123456",
            Email = "237183141@qq.com",
            Phone = "18516328675"
        };
        [TestMethod]
        public void TestMethod1()
        {

            using (var connection = new SqlConnection(connectionStr))
            {
                for (int i = 0; i < 10000; i++)
                {
                    connection.Add<User>(user);
                }

            }

        }

       

        [TestInitialize]
        public void Init()
        {

        }
    }
}
