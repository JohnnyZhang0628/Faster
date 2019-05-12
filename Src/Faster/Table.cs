using System;
using System.Collections.Generic;
using System.Text;

namespace Faster
{
    public class Table
    {
        /// <summary>
        /// 表名称
        /// </summary>
        [FasterKey]
        public string Name { get; set; }

        public List<Column> Columns { get; set; } = new List<Column>();
    }
}
