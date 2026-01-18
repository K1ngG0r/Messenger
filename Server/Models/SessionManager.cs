namespace Server;

public class SessionManager
{
    private readonly Dictionary<string, int> _sessions;  // SessionKey -> UserId
    private readonly ApplicationDbContext _context;
    
    public SessionManager(ApplicationDbContext context)
    {
        _context = context;
        _sessions = new Dictionary<string, int>();
    }
    
    public string CreateSession(int userId)
    {
        var sessionKey = GenerateSessionKey();
        _sessions[sessionKey] = userId;
        return sessionKey;
    }
    
    public User? GetUserBySession(string sessionKey)
    {
        if (_sessions.TryGetValue(sessionKey, out var userId))
        {
            return _context.Users.Find(userId);
        }
        return null;
    }
    
    public bool IsValidSession(string sessionKey)
    {
        return _sessions.ContainsKey(sessionKey);
    }
    
    private string GenerateSessionKey()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("=", "")
            .Replace("+", "-")
            .Replace("/", "_");
    }
}