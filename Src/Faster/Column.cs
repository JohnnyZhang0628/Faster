using System;

namespace Faster
{
    public class Column
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// 列类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 是否为主键
        /// </summary>
        public bool Key { get; set; } = false;
        /// <summary>
        /// 是否为自增长
        /// </summary>
        public bool Identity { get; set; } = false;
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Mark { get; set; }
    }
}
