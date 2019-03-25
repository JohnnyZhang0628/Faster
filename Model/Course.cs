using System;
using System.Collections.Generic;
using System.Text;
using Faster;

namespace Model
{
    [FasterTable(TableName ="tb_course")]
   public class Course
    {
        [FasterKey]
        [FasterIdentity]
        public int CourseId { get; set; }

        public string CourseName { get; set; }

        public string ClassRoom { get; set; }

        public DateTime StartDateTime { get; set; }
    }
}
