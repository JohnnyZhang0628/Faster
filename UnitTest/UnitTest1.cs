using Microsoft.VisualStudio.TestTools.UnitTesting;
using Faster;
using Model;
using System.Data;
using System.Data.SqlClient;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
     
        [TestMethod]
        public void TestMethod1()
        {
            using (IDbConnection connection = new SqlConnection())
            {
                //connection.GetList<User>();
                connection.Get<User>();
            }
        }

        [TestInitialize]
        public void Init()
        {
            
        }
    }
}
