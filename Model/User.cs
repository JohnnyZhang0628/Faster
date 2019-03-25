using Faster;

namespace Model
{
    [FasterTable(TableName = "tb_user")] //自动映射表的别名
    public class User
    {
        [FasterIdentity] //自增长ID
        [FasterKey] //设为主键
        public int UserId { get; set; }

        [FasterColumn(ColumnName = "user_name")] //设置列的别名
        [FasterKey] //多个主键
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }
}
