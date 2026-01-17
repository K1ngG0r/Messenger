using System.Net;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server;
public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string HashPassword {get;set; } = string.Empty;
    
    public virtual ICollection<SingleChange>? UnreadUpdate { get; set; }
}