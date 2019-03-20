using Faster;
using System;

namespace Model
{
   [FasterTable(TableName = "tb_user")]
    public class User
    {
        [FasterKey]
        public int UserId { get; set; }
        [FasterKey]
        [FasterColumn(ColumnName ="用户名")]
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }
}
