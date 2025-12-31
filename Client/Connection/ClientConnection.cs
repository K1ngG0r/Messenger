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
        public event Action<Response> NewResponse = null!;
        public ClientConnection(IPEndPoint serverIP) 
        {
            return;
            connectedServer = serverIP;
            udpConnection = new UdpConnection(1234);
            udpConnection.DataReceived += HandleMessage;
        }
        private void HandleMessage(byte[] bytes, IPEndPoint who)
        {
            return;
            if (connectedServer.ToString() != who.ToString())
                return;
            string messageString = Encoding.UTF8.GetString(bytes);
            Response? response = JsonSerializer.Deserialize<Response?>(messageString);
            if (response == null)
                return;
            NewResponse?.Invoke(response);
        }
        public async Task SendAsync(Request request)
        {
            return;
            await udpConnection.SendAsync(
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request)),
                connectedServer);
        }
    }
}
