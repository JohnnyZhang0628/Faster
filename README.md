# Faster
## 基于Dapper的ORM框架，更小、更快是作者追求的目标。
## 邮箱：237183141@qq.com
## 版本
### V1.0.0.1 完成基本的增删改查。
### V1.0.0.2 新增分页查询，仓储和服务为以后手写IOC做准备
### V1.0.0.3 新增DB First和Code First两种模式。
## 基本的单表的CURD
``` C#
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
            var _dbConnection = new SqlConnection("server=.;database=test;user id=sa;password=55969126");

            //Code First 根据model生成表
            string modelPath = @"E:\WorkSpace\Faster\Model\bin\Debug\netstandard2.0\Model.dll";
            _dbConnection.CreateTable(modelPath);

            //DB First 根据数据库生成model(当前项目debug 下面的Model文件夹)
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
	// 用户接口
	  public interface IUserRepository:IRepository
    {
	// 自定义业务逻辑
        bool Login(string username, string password);
    }
	// 基本服务类实现基本接口
	 public class BaseService : IRepository
    {
        private IDbConnection _dbConnection { set; get; }

        private const string _connectionStr = "server=.;database=test;user id=sa;password=55969126";

        public BaseService()
        {
            _dbConnection = new SqlConnection(_connectionStr);
        }

        public int Add<T>(IEnumerable<T> modelList)
        {
            return _dbConnection.Add<T>(modelList);
        }

        public T Get<T>(params object[] param)
        {
            return _dbConnection.Get<T>(param);
        }

        public IEnumerable<T> GetList<T>(string strWhere = "",object param=null)
        {
            return _dbConnection.GetList<T>(strWhere,param);
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