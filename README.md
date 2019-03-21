# Faster
## 基于Dapper的ORM框架，更小、更快是作者追求的目标。
## 版本
### V1.0.0.1 完成基本的增删改查。
## 基本的单表的CURD
``` C#
[FasterTable(TableName = "tb_user")] //自动映射表的别名
    public class User
    {
        [FasterKey] //设为主键
        public int UserId { get; set; }
        [FasterColumn(ColumnName ="user_name")] //设置列的别名
        [FasterKey] //多个主键
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }

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
				//多主键查询
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

                //多主键删除

                var deleteRow = connection.Remove<User>(2, "张强1");
            }
```