namespace Server;

public class UserSession
{
    public string SessionKey { get; set; }
    public User User { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public UserSession(string sessionKey, User user)
    {
        SessionKey = sessionKey;
        User = user;
        CreatedAt = DateTime.UtcNow;
    }
}