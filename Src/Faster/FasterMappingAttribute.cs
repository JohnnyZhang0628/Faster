using System;
using System.Collections.Generic;
using System.Text;

namespace Faster
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class FasterTableAttribute : Attribute
    {
        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class FasterColumnAttribute : Attribute
    {

        /// <summary>
        /// 列名称
        /// </summary>
        public string ColumnName { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class FasterKeyAttribute : Attribute
    {
        /// <summary>
        /// 主键
        /// </summary>
        public bool Key { get; set; } = true;
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class FasterIdentityAttribute : Attribute
    {
        /// <summary>
        /// 自增长
        /// </summary>
        public bool Identity { get; set; } = true;
    }

}
