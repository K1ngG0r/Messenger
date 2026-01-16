using System.Net;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server;
public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public IPAddress LocalServerAddress { get; set; } = IPAddress.Parse("127.0.0.1");
    public int LocalServerPort { get; set; }
    public DateTime LastSeen { get; set; }
    
    public virtual ICollection<Message>? UnreadMessages { get; set; }
    
    [NotMapped]
    public IPEndPoint LocalServer => 
        new IPEndPoint(LocalServerAddress, LocalServerPort);

}