# Faster
## 基于Dapper的ORM框架，更小、更快是作者追求的目标。
## 版本
### V1.0.0.1 完成基本的增删改查。
## 基本的单表的CURD
``` C#
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

                var user = connection.Get<User>(1);

                //修改
                var updateRow = connection.Update<User>(new User
                {
                    UserId = 1,
                    UserName = "zq",
                    Password = "zq",
                    Email = "zq@qq.com",
                    Phone = "zq"
                });

                //删除

                var deleteRow = connection.Remove<User>(10000);
            }
```