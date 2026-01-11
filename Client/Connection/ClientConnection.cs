using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Core;
using Client.Models;

namespace Client.Connection
{
    public class ClientConnection
    {
        private string sessionKey = string.Empty;
        private IPEndPoint connectedServer;
        private UdpConnection udpConnection;
        private Dictionary<Guid, TaskCompletionSource<Response>> _pendingRequests = new();
        private object _lock = new();
        public ClientConnection(IPEndPoint serverIP)
        {
            connectedServer = serverIP;
            udpConnection = new UdpConnection(1234);
            udpConnection.DataReceived += HandleMessage;
        }
        public async Task<Response> SendAsync(RequestMethod method, string body, TimeSpan timeout)
        {
            var correlationId = Guid.NewGuid();
            var request = new Request(sessionKey, correlationId, method, body);
            var tcs = new TaskCompletionSource<Response>();
            lock (_lock)
            {
                _pendingRequests[correlationId] = tcs;
            }
            try
            {
                var requestBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
                await udpConnection.SendAsync(requestBytes, connectedServer);
                using (var cts = new CancellationTokenSource(timeout))
                {
                    return await tcs.Task.WaitAsync(cts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException();
            }
            catch
            {
                RemovePendingRequest(correlationId);
                throw new Exception();
            }
        }
        public async Task<Response> SendAsync(RequestMethod method, string body)
        {
            return await SendAsync(method, body, TimeSpan.FromSeconds(2));
        }
        private void HandleMessage(byte[] bytes, IPEndPoint who)
        {
            if (connectedServer.ToString() != who.ToString())
                return;
            string messageString = Encoding.UTF8.GetString(bytes);
            Response? response = JsonSerializer.Deserialize<Response?>(messageString);
            if (response == null)
                return;
            TaskCompletionSource<Response>? tcs;
            lock (_lock)
            {
                if (!_pendingRequests.TryGetValue(response.CorrelationId, out tcs))
                    return;
                _pendingRequests.Remove(response.CorrelationId);
            }
            tcs.TrySetResult(response);
        }
        private void RemovePendingRequest(Guid id)
        {
            lock (_lock)
            {
                if (_pendingRequests.Remove(id, out var tcs))
                {
                    tcs.TrySetException(new Exception());
                }
            }
        }
    }
}
