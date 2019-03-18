using Microsoft.VisualStudio.TestTools.UnitTesting;
using Faster;
using Model;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        private Core _core;
        [TestMethod]
        public void TestMethod1()
        {
            _core.GetAll<User>();
            var user = new User
            {
                UserId = 1,
                UserName = "zq",
                Password = "123456",
                Phone = "18516328675"
            };
            _core.Create(user);
           
        }

        [TestInitialize]
        public void Init()
        {
            _core = new Core();
        }
    }
}
