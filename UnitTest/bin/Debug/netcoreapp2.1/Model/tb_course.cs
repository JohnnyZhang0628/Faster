using Faster;

namespace Model
{
    public class tb_course
    {
        /// <summary>
///  
/// </summary>
[FasterKey]
[FasterIdentity]
public int CourseId { get; set; }  
/// <summary>
///  
/// </summary>
public string CourseName { get; set; }  
/// <summary>
///  
/// </summary>
public string ClassRoom { get; set; }  
/// <summary>
///  
/// </summary>
public DateTime StartDateTime { get; set; }  

    }
    
}
