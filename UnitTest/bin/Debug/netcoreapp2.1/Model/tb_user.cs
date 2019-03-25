using Faster;

namespace Model
{
    public class tb_user
    {
        /// <summary>
///  
/// </summary>
[FasterKey]
[FasterIdentity]
public int UserId { get; set; }  
/// <summary>
///  
/// </summary>
[FasterKey]
public string user_name { get; set; }  
/// <summary>
///  
/// </summary>
public string Password { get; set; }  
/// <summary>
///  
/// </summary>
public string Email { get; set; }  
/// <summary>
///  
/// </summary>
public string Phone { get; set; }  

    }
    
}
