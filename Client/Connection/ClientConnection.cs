using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client.Connection
{
    public class ClientConnection
    {
        private UdpServer server;
        public ClientConnection() 
        {
            server = new UdpServer(1234, new NullPresentationService());
            server.OnReceive += HandleMessage;
        }
        private void HandleMessage(string message)
        {
            Request? request = JsonSerializer.Deserialize<Request?>(message);
            if (request == null)
                return;
        }
    }
}
