# Faster
## 基于.net standard 2.0 的ORM框架，更小、更快是作者追求的目标。
## 邮箱：237183141@qq.com
## 大小
### 50kb
## 特性
### 1、支持单表的增删改查、分页。
### 2、所有查询全部参数化，防止sql注入。
### 3、支持db first 模式，直接生成model类。code first作者舍弃了。
### 4、支持IOC，依赖注入。
## 接口设计模式参考dapper
## 问题
### Q:dapper和faster的区别？
### A:dapper是一款很好的ORM框架，作者开始就是用它的。随着需求的改变（根据实体直接反射数据库，所有sql语句参数化查询，db first，ioc等）
### dapper已经不能满足我的需求了，所以就出现faster。如果你没有上诉我的这些需求，我推荐你用dapper。如果你需要一些定制化的需求，faster是你
### 的不二之选。


## 基本的单表的CURD
``` 
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

            //查询多个数据集
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" select  * from tb_user where userid >= 10");
            strSql.Append(" select  * from tb_user where userid >= 100");
            var multiple = _dbConnection.ExecuteQueryMultiple<User>(strSql.ToString());

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
            IDbDataParameter[]  outparameters =
            {
                new SqlParameter { ParameterName = "@count",DbType=DbType.Int32, Direction = ParameterDirection.Output }
            };

            _dbConnection.ExecuteNonQuerySP("sp_test_out", outparameters);


            var count = outparameters[0].Value;
        }
    }
```
