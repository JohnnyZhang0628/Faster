using Faster;

namespace Model
{
    public class tb_test
    {
        /// <summary>
///  
/// </summary>
[FasterKey]
[FasterIdentity]
public int id { get; set; }  
/// <summary>
///  
/// </summary>
public DateTime date { get; set; }  
/// <summary>
///  
/// </summary>
public string city { get; set; }  
/// <summary>
///  
/// </summary>
public int order_num { get; set; }  
/// <summary>
///  
/// </summary>
public double percent { get; set; }  

    }
    
}
