using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Client.Models;

namespace Client.Connection
{
    public class ClientConnection
    {
        private IPEndPoint connectedServer;
        private UdpConnection udpConnection;
        public event Action<Chat, ChatMessage> NewMessage;
        public ClientConnection() 
        {
            udpConnection = new UdpConnection(1234, new NullPresentationService());
            udpConnection.OnReceive += HandleMessage;
        }
        private void HandleMessage(string message)
        {
            Request? request = JsonSerializer.Deserialize<Request?>(message);
            if (request == null)
                return;

        }
        public void SendMessage(Chat to, ChatMessage message)
        {

        }
    }
}
