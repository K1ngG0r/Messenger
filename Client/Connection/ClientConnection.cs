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
        private IPresentationService _ps;
        //private TcpConnection tcpConnection;
        private Dictionary<Guid, TaskCompletionSource<Response>> _pendingRequests = new();
        private object _lock = new();
        public ClientConnection(IPEndPoint serverIP, IPresentationService ps)
        {
            connectedServer = serverIP;
            udpConnection = new UdpConnection(1234);
            udpConnection.Start();
            udpConnection.DataReceived += HandleMessage;
            _ps = ps;
        }
        public async Task<(UserSettings, List<Guid>)> Login(string username, string password)
        {
            _ps.DisplayMessage($"логин под {username} {password}");
            var body = JsonSerializer.Serialize(
                new LoginRequestSettings(username, password));
            try
            {
                var response = await SendAndVerifyAsync(RequestMethod.Login, body);
                var loginSettings = JsonSerializer.Deserialize
                    <LoginResponseSettings>(response.Payload);
                if (loginSettings is null)
                    throw new Exception();
                sessionKey = loginSettings.sessionKey;
                _ps.DisplayMessage($"успешный вход {sessionKey}");
                return (loginSettings.settings, loginSettings.chats);
            }
            catch
            {
                _ps.DisplayMessage($"не удалось залогиниться");
                throw new Exception();
            }
        }
        public void Logout()
        {
            sessionKey = string.Empty;
        }
        public async Task SendMessage(Guid chatId, string message)
        {
            _ps.DisplayMessage($"send {message}");
            var body = JsonSerializer.Serialize(
                new SendRequestSettings(chatId, message));
            try
            {
                var response = await SendAndVerifyAsync(RequestMethod.Send, body);
                _ps.DisplayMessage($"send succesful");
            }
            catch
            {
                _ps.DisplayMessage($"send failed");
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
        public async Task<Guid> CreateChat(CreateChatRequestSettingsMethod chatType, string chatParameter)
        {
            _ps.DisplayMessage($"create chat");
            var body = JsonSerializer.Serialize(
                new CreateChatRequestSettings(chatType, chatParameter));
            try
            {
                var response = await SendAndVerifyAsync(RequestMethod.CreateChat, body);
                if (!Guid.TryParse(response.Payload, out var chatId))
                    throw new Exception();
                _ps.DisplayMessage($"create chat succesful {chatId}");
                return chatId;
            }
            catch
            {
                _ps.DisplayMessage($"create chat failed");
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
                var userinfo = JsonSerializer.Deserialize<UserInfo>(response.Payload);
                if (userinfo is null)
                    throw new Exception();
                return new User(userinfo.name, userinfo.username,
                    CacheManager.SetUserAvatarPathByUsername(userinfo.username, userinfo.avatar));
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
                    return Task.Run(async () => await tcs.Task.WaitAsync(cts.Token)).Result;
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
            return await SendAsync(method, body, TimeSpan.FromSeconds(30));
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
        /*private void HandleMessage(string messageString, EndPoint who)
        {
            if (connectedServer.ToString() != who.ToString())
                return;
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
        }*/
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
