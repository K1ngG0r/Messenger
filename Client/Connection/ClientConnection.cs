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
using Client.Connection.Pattenrs;
using Client.Models;

namespace Client.Connection
{
    public class ClientConnection
    {
        private string sessionKey = string.Empty;
        private IPEndPoint connectedServer;
        private UdpConnection udpConnection;
        private IPresentationService _ps;
        private Dictionary<Guid, TaskCompletionSource<Response>> _pendingRequests = new();
        private object _lock = new();
        public ClientConnection(IPEndPoint serverIP, IPresentationService ps)
        {
            connectedServer = serverIP;
            udpConnection = new UdpConnection(0);
            udpConnection.Start();
            udpConnection.DataReceived += HandleMessage;
            _ps = ps;
        }
        public async Task<Result<LoginResponseSettings>> Login(string username, string password)
        {
            var body = JsonSerializer.Serialize(
                new LoginRequestSettings(username, password));
            var response = await SendAndVerifyAsync(RequestMethod.Login, body);
            if(response.IsFailed)
                return Result<LoginResponseSettings>.Failure();
            var loginSettings = JsonSerializer.Deserialize
                <LoginResponseSettings>(response.Value!.Payload);
            if (loginSettings is null)
                return Result<LoginResponseSettings>.Failure();
            return Result<LoginResponseSettings>.Success(loginSettings);
        }
        public async Task<Result> SendMessage(Guid chatId, string message)
        {
            var body = JsonSerializer.Serialize(
                new SendRequestSettings(chatId, message));
            var response = await SendAndVerifyAsync(RequestMethod.Send, body);
            if(response.IsFailed)
                return Result.Failure();
            return Result.Success();
        }
        public async Task<Result<List<SingleChange>>> Update()
        {
            var response = await SendAndVerifyAsync(RequestMethod.Update, string.Empty);
            if (response.IsFailed)
                return Result<List<SingleChange>>.Failure();
            var changes = JsonSerializer.Deserialize<List<SingleChange>>(response.Value!.Payload);
            if (changes is null)
                return Result<List<SingleChange>>.Failure();
            return Result<List<SingleChange>>.Success(changes);
        }
        public async Task<Result<Guid>> CreatePrivateChat(string username)
        {
            return await CreateChat(CreateChatRequestSettingsMethod.PrivateChat, username);
        }
        private async Task<Result<Guid>> CreateChat(CreateChatRequestSettingsMethod chatType, string chatParameter)
        {
            var body = JsonSerializer.Serialize(
                new CreateChatRequestSettings(chatType, chatParameter));
            var response = await SendAndVerifyAsync(RequestMethod.CreateChat, body);
            if(response.IsFailed)
                return Result<Guid>.Failure();
            if (!Guid.TryParse(response.Value!.Payload, out var chatId))
                return Result<Guid>.Failure();
            return Result<Guid>.Success(chatId);
        }
        public async Task<Result<Models.UserInfo>> LoadUser(string username)
        {
            var body = JsonSerializer.Serialize(
                new LoadRequestSettings(LoadRequestSettingsMethod.User, username));
            var response = await SendAndVerifyAsync(RequestMethod.Load, body);
            if (response.IsFailed)
                return Result<Models.UserInfo>.Failure();
            var userinfo = JsonSerializer.Deserialize<UserInfo>(response.Value!.Payload);
            if (userinfo is null)
                return Result<Models.UserInfo>.Failure();
            return Result<Models.UserInfo>.Success(new Models.UserInfo(userinfo.name, userinfo.username,
                CacheManager.SetUserAvatarPathByUsername(userinfo.username, userinfo.avatar)));
        }
        public async Task<Result<Chat>> LoadChat(Guid chatId)
        {
            var body = JsonSerializer.Serialize(
                new LoadRequestSettings(LoadRequestSettingsMethod.Chat, chatId.ToString()));
            var response = await SendAndVerifyAsync(RequestMethod.Load, body);
            if (response.IsFailed)
                return Result<Chat>.Failure();
            return Result<Chat>.Success(new PrivateChat());//fixit
        }
        private async Task<Result<Response>> SendAndVerifyAsync(RequestMethod method, string body)
        {
            try
            {
                var response = await SendAsync(method, body);
                if (response.IsFailed)
                    return Result<Response>.Failure();
                if (!(response.Value!.Code is ResponseStatusCode.Ok))
                    return Result<Response>.Failure();
                return response;
            }
            catch
            {
                return Result<Response>.Failure();
            }
        }
        private async Task<Result<Response>> SendAsync(RequestMethod method, string body, TimeSpan timeout)
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
                    return Result<Response>.Success(await Task.Run(async () => await tcs.Task.WaitAsync(cts.Token)));
                }
            }
            catch
            {
                return Result<Response>.Failure();
            }
            finally
            {
                RemovePendingRequest(correlationId);
            }
        }
        private async Task<Result<Response>> SendAsync(RequestMethod method, string body)
        {
            return await SendAsync(method, body, TimeSpan.FromSeconds(3));
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
