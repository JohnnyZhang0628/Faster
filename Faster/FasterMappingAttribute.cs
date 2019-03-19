using System;
using System.Collections.Generic;
using System.Text;

namespace Faster
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = true)]
    public class FasterMappingAttribute : Attribute
    {
        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 列名称
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
        public bool Key { get; set; }


    }
}
