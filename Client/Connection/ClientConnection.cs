using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
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
            udpConnection.Start();
            udpConnection.DataReceived += HandleMessage;
        }
        public async Task Login(string username, string password)
        {
            var body = JsonSerializer.Serialize(
                new LoginRequestSettings(username, password));
            try
            {
                var response = await SendAndVerifyAsync(RequestMethod.Login, body);
                sessionKey = response.Payload;
            }
            catch
            {
                throw new Exception();
            }
        }
        public async Task SendMessage(Guid chatId, string message)
        {
            var body = JsonSerializer.Serialize(
                new SendRequestSettings(chatId, message));
            try
            {
                var response = await SendAndVerifyAsync(RequestMethod.Send, body);
            }
            catch
            {
                throw new Exception();
            }
        }
        public async Task<List<SingleChange>> Update()
        {
            try
            {
                var response = await SendAndVerifyAsync(RequestMethod.Update, string.Empty);
                var changes = JsonSerializer.Deserialize<List<SingleChange>>(response.Payload);
                if (changes is null)
                    throw new Exception();
                return changes;
            }
            catch
            {
                throw new Exception();
            }
        }
        public async Task<Guid> CreateChat(CreateChatRequestSettingsMethod chatType)
        {
            var body = JsonSerializer.Serialize(
                new CreateChatRequestSettings(chatType, string.Empty));//fixit
            try
            {
                var response = await SendAndVerifyAsync(RequestMethod.CreateChat, body);
                if (!Guid.TryParse(response.Payload, out var chatId))
                    throw new Exception();
                return chatId;
            }
            catch
            {
                throw new Exception();
            }
        }
        public async Task<User> LoadUser(string username)
        {
            var body = JsonSerializer.Serialize(
                new LoadRequestSettings(LoadRequestSettingsMethod.User, username));
            try
            {
                var response = await SendAndVerifyAsync(RequestMethod.Load, body);
                return new User();
            }
            catch
            {
                throw new Exception();
            }
        }
        public async Task<Chat> LoadChat(Guid chatId)
        {
            var body = JsonSerializer.Serialize(
                new LoadRequestSettings(LoadRequestSettingsMethod.Chat, chatId.ToString()));
            try
            {
                var response = await SendAndVerifyAsync(RequestMethod.Load, body);
                return new PrivateChat();//fixit
            }
            catch
            {
                throw new Exception();
            }
        }
        private async Task<Response> SendAndVerifyAsync(RequestMethod method, string body)
        {
            try
            {
                var response = await SendAsync(method, body);
                if (!(response.Code is ResponseStatusCode.Ok))
                    throw new VerificationException();
                return response;
            }
            catch(VerificationException)
            {
                throw new VerificationException();
            }
            catch
            {
                throw new Exception();
            }
        }
        private async Task<Response> SendAsync(RequestMethod method, string body, TimeSpan timeout)
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
                udpConnection.Send(requestBytes, connectedServer);
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
        private async Task<Response> SendAsync(RequestMethod method, string body)
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
