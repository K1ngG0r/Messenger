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
        public event Action<Chat, ChatMessage> NewMessage = null!;
        public event Action<Chat, ChatMessage> NewChat = null!;
        public ClientConnection(IPEndPoint serverIP) 
        {
            connectedServer = serverIP;
            udpConnection = new UdpConnection(1234, new NullPresentationService());
            udpConnection.DataReceived += HandleMessage;
        }
        private void HandleMessage(string message)
        {
            Request? request = JsonSerializer.Deserialize<Request?>(message);
            if (request == null)
                return;

        }
        public async Task SendMessageAsync(Chat to, ChatMessage message)
        {
            //await udpConnection.SendAsync()
        }
    }
}
