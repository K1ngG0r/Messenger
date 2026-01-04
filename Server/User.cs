using System.Net;

namespace Server
{
    public class User
    {
        public string? key;
        
        public string? userName;

        public EndPoint? localServer;

        public List<Message>? unreadMessage = new List<Message>();

        public User(string key,string userName, IPEndPoint localServer)
        {
            this.key = key;
            this.userName = userName;
            this.localServer = localServer;
        }
    }
}