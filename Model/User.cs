using Faster;
using System;

namespace Model
{
   // [FasterMapping(TableName = "tb_user")]
    public class User
    {
        [FasterMapping(Key = true)]
        public int UserId { get; set; }

        [FasterMapping(ColumnName ="用户名")]
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }
}
