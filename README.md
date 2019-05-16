# Faster
## 基于.net standard 2.0 的ORM框架，更小、更快是作者追求的目标。
## 邮箱：237183141@qq.com
## 大小：50kb
## 特性
### 1、支持单表的增删改查、分页、批量新增、批量删除。
### 2、所有查询全部参数化，防止sql注入。
### 3、支持db first 模式，直接生成model类。code first作者舍弃了。
### 4、支持IOC，依赖注入。
### 5、支出dynamic动态类型。
### 6、所有操作都是以事务进行的。
## 接口设计模式参考dapper
## 问题
### Q:dapper和faster的区别？
### A:dapper是一款很好的ORM框架，作者开始就是用它的。随着需求的改变（根据实体直接反射数据库，所有sql语句参数化查询，db first，ioc等）
### dapper已经不能满足我的需求了，所以就出现faster。如果你没有上诉我的这些需求，我推荐你用dapper。如果你需要一些定制化的需求，faster是你
### 的不二之选。


## Example
### 所有的方法都基于IDbConnection这个接口。
### User实体类
```
    [FasterTable(TableName = "tb_user")] //自动映射表的别名
    public class User
    {

        [FasterIdentity] //自增长ID
        [FasterKey] //设为主键
        public int UserId { get; set; }


        [FasterColumn(ColumnName = "user_name")] //设置列的别名
        [FasterKey] //多个主键
        public string UserName { get; set; } = "zq";

        public string Password { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }
```
### 1、获取实体列表。IEnumerable<T> GetList<T>(this IDbConnection connection, string strWhere = "", object param = null)
```
	connection.GetList<User>(" where userid>@id", new { id = 10 })
```
### 2、根据主键加载单个实体。T Get<T>(this IDbConnection connection, object param ) 
```
	connection.Get<User>(new { UserId = 1, UserName = "张强1" });
```
### 3、新增实体。int Add<T>(this IDbConnection connection, object param)
```
	connection.Add(new User
                {
                    UserName = "张强",
                    Password = "123456",
                    Email = "237183141@qq.com",
                    Phone = "18516328675"
                });
````
### 4、批量新增实体。int BulkInsert<T>(this IDbConnection connection, IEnumerable<T> param)
### BulkInsert 会生产一条语句，一次执行。这是和Dapper的区别。Dapper会遍历一次，执行一次。
```
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
            connection.BulkInsert(userList);
			// 反射生产的sql语句,然后执行一次，大大提升了批量插入的效率。
			insert into tb_user(user_name,password,email,phone)values(@username0,@password0,@email0,@phone0);
			insert into tb_user(user_name,password,email,phone)values(@username1,@password1,@email1,@phone1);
			insert into tb_user(user_name,password,email,phone)values(@username2,@password2,@email2,@phone2);
			.........
			insert into tb_user(user_name,password,email,phone)values(@username9999,@password9999,@email9999,@phone9999);

```
### 5、根据主键更新实体。int Update<T>(this IDbConnection connection, object param)
```
	connection.Update<User>(new { UserId = 1, UserName = "张强1" });
```
### 6、根据主键删除实体。int Remove<T>(this IDbConnection connection,object param)
```
	connection.Remove<User>(new { UserId = 1, UserName = "张强1" });
```
### 7、根据主键批量删除实体。int BulkRemove<T>(this IDbConnection connection, IEnumerable<T> param)
### 类似批量新增，生成一条语句，提高效率。
```
			List<User> userList = new List<User>();
            for (int i = 0; i < 100; i++)
            {
                userList.Add(new User
                {
                    UserName = "张强" + (i + 1),
                    Password = "123456",
                    Email = "237183141@qq.com",
                    Phone = "18516328675"
                });
            }
	connection.BulkRemove(userList);
```

### 8、分页查询。Tuple<int, IEnumerable<T>> GetPageList<T>(this IDbConnection connection, string order, string strWhere = "", object param = null, int pageNum = 1, int PageSize = 10) 
```
	var result=connection.GetPageList<User>("userid,username desc"," where userid>@id",new {id=10},2,20);
	//当前符合条件的记录数
	int count=result.Item1;
	// 第21条-40条 实体列表
	var list=result.Item2;
```
## 执行sql语句
### 1、获取第一行第一列的结果。T GetValue<T>(this IDbConnection connection, string strSql, object param = null)
```
	connection.GetValue<int>("select count(*) from tb_user where userid>@userid",new {userid=10});
```
### 2、获取数据集。IEnumerable<T> ExecuteQuery<T>(this IDbConnection connection, string strSql, object param = null)
```
	connection.ExecuteQuery<User>("select * from tb_user where userid>@userid",new {userid=10});
```

### 3、获取动态类型数据集。IEnumerable<dynamic> ExecuteQueryDynamic(this IDbConnection connection, string strSql, object param = null)
```
	connection.ExecuteQueryDynamic("select * from tb_user where userid>@userid",new {userid=10});
```

### 4、执行修改命令语句。int ExecuteNonQuery(this IDbConnection connection, string strSql, object param = null)
```
	connection.ExecuteNonQuery("delete from tb_user where userid>@userid",new {userid=10});
```
### 5、执行无返回值的存储过程。int ExecuteNonQuerySP(this IDbConnection connection, string storeProcedure, IDbDataParameter[] parameters = null)
```
            IDbDataParameter[] parameters =
            {
                new SqlParameter("@user_id",2)
            };
            query = _dbConnection.ExecuteQuerySP<User>("sp_test", parameters);

			// 获取返回值
            IDbDataParameter[]  outparameters =
            {
                new SqlParameter { ParameterName = "@count",DbType=DbType.Int32, Direction = ParameterDirection.Output }
            };

            _dbConnection.ExecuteNonQuerySP("sp_test_out", outparameters);


            var count = outparameters[0].Value;
```

### 6、执行查询存储过程。IEnumerable<T> ExecuteQuerySP<T>(this IDbConnection connection, string storeProcedure, IDbDataParameter[] parameters = null)
```
	connection.ExecuteQuerySP<User>("sp_test_no_params");
```
## DB First 生成Model类 void CreateModels(this IDbConnection connection, string nameSpace = "Model")
### connection.CreateModels();

## IOC 容易依赖注入
### 
```
            //1、获取容器
            Container container = new Container();
            //2、注册类型
            container.RegisterType<IUserRepository, UserService>();
            //3、创建实例
            user = container.Resolve<IUserRepository>();

```


