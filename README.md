# Faster
## 基于Dapper的ORM框架，更小、更快是作者追求的目标。
## 邮箱：237183141@qq.com
## 感谢ELEVEN的指导。
## 版本
### V1.0.0.1 完成基本的增删改查。
### V1.0.0.2 新增分页查询，仓储和服务为以后手写IOC做准备
### V1.0.0.3 新增DB First和Code First两种模式。
### V1.0.0.4 新增IOC容器，依赖注入
## 基本的单表的CURD
``` C#
	
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
	// IOC注入容器类
	 public class Container
    {
        private static Dictionary<string, Type> cacheDic = new Dictionary<string, Type>();
        /// <summary>
        /// 注册类型
        /// </summary>
        /// <typeparam name="IT">抽象类</typeparam>
        /// <typeparam name="T">抽象实现类</typeparam>
        public void RegisterType<IT, T>()
        {
            //设置缓存
            cacheDic.Add(typeof(IT).FullName, typeof(T));
        }

        /// <summary>
        /// 创建类型
        /// </summary>
        /// <typeparam name="IT"></typeparam>
        /// <returns></returns>
        public IT Resolve<IT>()
        {
            string key = typeof(IT).FullName;
            Type type = (Type)cacheDic[key];
            object oValue = Create(type);
            return (IT)oValue;
        }

        private object Create(Type type)
        {
            //优先标记特性，就找参数个数最多的
            var ctorArray = type.GetConstructors();
            ConstructorInfo ctor = null;
            if (ctorArray.Where(c => c.IsDefined(typeof(InjectionConstructorAttribute), true)).Count() > 0)
            {
                ctor = ctorArray.Where(c => c.IsDefined(typeof(InjectionConstructorAttribute), true)).FirstOrDefault();
            }
            else
            {
                ctor = ctorArray.OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();
            }
            var paraArray = ctor.GetParameters();
            if (paraArray.Length == 0)
            {
                return Activator.CreateInstance(type);
            }
            List<object> listPara = new List<object>();
            foreach (var para in paraArray)
            {
                string keyType = para.ParameterType.FullName;
                if (cacheDic.ContainsKey(keyType))
                {
                    object oPara = Create(cacheDic[keyType]);//这里递归的
                    listPara.Add(oPara);
                }
                else
                    throw new Exception($"please first register {keyType} type");
            }
            return Activator.CreateInstance(type, listPara.ToArray());
        }
    }
	[FasterTable(TableName = "tb_user")] //自动映射表的别名
    public class User
    {
		[FasterIdentity] //自增长ID
        [FasterKey] //设为主键
        public int UserId { get; set; }
        [FasterColumn(ColumnName ="user_name")] //设置列的别名
        [FasterKey] //多个主键
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }
	// 基本增删改查接口
	public interface IRepository
    {
        IEnumerable<T> GetList<T>(string strWhere = "",object param=null);
        T Get<T>(params object[] param);

        int Add<T>(IEnumerable<T> modelList);

        int Update<T>(IEnumerable<T> modelList);
        int Remove<T>(params object[] param);
        Tuple<int, IEnumerable<T>> GetPageList<T>(string order, string strWhere = "", object param = null, int pageNum = 1, int PageSize = 10);

    }
	public abstract class BaseService : IRepository
    {
        // 静态构造函数实现单例
        public static IDbConnection _dbConnection;

        static BaseService()
        {
            _dbConnection = new SqlConnection("server=.;database=test;user id=sa;password=55969126");
        }

        public int Add<T>(IEnumerable<T> modelList)
        {
            return _dbConnection.Add<T>(modelList);
        }

        public T Get<T>(params object[] param)
        {
            return _dbConnection.Get<T>(param);
        }

        public IEnumerable<T> GetList<T>(string strWhere = "", object param = null)
        {
            return _dbConnection.GetList<T>(strWhere, param);
        }

        public int Update<T>(IEnumerable<T> modelList)
        {
            return _dbConnection.Update<T>(modelList);
        }

        public int Remove<T>(params object[] param)
        {

            return _dbConnection.Remove<T>(param);
        }

        public Tuple<int, IEnumerable<T>> GetPageList<T>(string order, string strWhere = "", object param = null, int pageNum = 1, int PageSize = 10)
        {
            return _dbConnection.GetPageList<T>(order, strWhere, param, pageNum, PageSize);
        }
    }
	// 用户继承接口和基本实现类
	public class UserService : BaseService, IUserRepository
    {
        public bool Login(string username, string password)
        {
            return true;
        }
    }

  
```