using System;
using System.IO;
using System.Windows;
using Client.Connection;
using Client.Connection.Pattenrs;
using Client.Data;
using Client.Models;
using Microsoft.EntityFrameworkCore;

namespace Client.ViewModels.Patterns.Services
{
    public class ChatService
    {
        private ClientConnection _connection;
        private AppDBContext _context;
        private CancellationTokenSource? _pollingCts;
        public void DeleteMessage(int messageId)
        {
            var messageToDelete = _context.Messages
                .First(x => x.Id == messageId);
            _context.Remove(messageToDelete);
            _context.SaveChangesAsync();
        }
        public void DeleteAllMessages(int chatId)
        {
            var chat = _context.Chats.First(x => x.Id == chatId);
            var messagesToDelete = _context.Messages.Where(x => x.Chat == chat);
            _context.RemoveRange(messagesToDelete);
            _context.SaveChangesAsync();
        }
        public void DeleteChat(int chatId)
        {
            _context.Remove(_context.Chats.First(x => x.Id == chatId));
            _context.SaveChangesAsync();
        }
        public async Task<Result<Models.UserInfo>> TryLoadUserByUsername(string username)
        {
            var user = _context.Users.FirstOrDefault(x => x.Username == username);
            if (user is null)
            {
                var result = await _connection.LoadUser(username);
                if (result.IsFailed)
                    return Result<Models.UserInfo>.Failure();
                return Result<Models.UserInfo>.Success(result.Value!);
            }
            return Result<Models.UserInfo>.Success(user);
        }
        /*public void UpdateSettings(string name, string username, string avatarPath)
        {

        }*/
        public Result<Chat> TryLoadPrivateChatByUsername(string username)
        {
            var chat = _context.Chats
                .OfType<PrivateChat>()
                .Include(x => x.Messages)
                    .ThenInclude(x => x.Who)
                .FirstOrDefault(x => x.Correspondent.Username == username);
            if (chat is null)
                return Result<Chat>.Failure();
            return Result<Chat>.Success(chat);
        }
        public Result<Chat> TryLoadChat(int chatId)
        {
            var groupChat = _context.Chats.OfType<GroupChat>()
                .Include(x => x.Owner)
                .Include(x => x.Messages).ThenInclude(x => x.Who)
                .Include(x => x.Participants).ThenInclude(x => x.User)
                .FirstOrDefault(x => x.Id == chatId);
            if (groupChat != null) 
                return Result<Chat>.Success(groupChat);
            var channelChat = _context.Chats.OfType<ChannelChat>()
                .Include(x => x.Messages).ThenInclude(x => x.Who)
                .Include(x => x.Subscribers).ThenInclude(x => x.User)
                .FirstOrDefault(x => x.Id == chatId);
            if (channelChat != null)
                return Result<Chat>.Success(channelChat);
            var privateChat = _context.Chats.OfType<PrivateChat>()
                .Include(x => x.Correspondent)
                .Include(x => x.Messages).ThenInclude(x => x.Who)
                .FirstOrDefault(x => x.Id == chatId);
            if (privateChat != null)
                return Result<Chat>.Success(privateChat);
            return Result<Chat>.Failure();
        }
        public Result<List<Chat>> LoadChatsList()
        {
            try
            {
                var chats = _context.Chats.ToList();
                return Result<List<Chat>>.Success(chats);
            }
            catch
            {
                return Result<List<Chat>>.Failure();
            }
        }
        public async Task<ChatMessage> SendMessage(ChatMessage message)
        {
            var sendResult = await _connection.SendMessage(message.Chat.ChatId, message.Message);
            if(sendResult.IsFailed)
                message.State = ChatMessage.ChatMessageState.NotDelivered;
            else
                message.State = ChatMessage.ChatMessageState.Delivered;
            message = _context.Messages.Add(message).Entity;
            _context.SaveChanges();
            return message;
        }
        public async Task<Result<Chat>> CreateNewChat(Chat chat)
        {
            Guid chatId = default;
            switch (chat)
            {
                case PrivateChat privateChat:
                    var chatResult = await _connection.CreatePrivateChat(
                        privateChat.Correspondent.Username);
                    if (chatResult.IsFailed)
                        return Result<Chat>.Failure();
                    chatId = chatResult.Value!;
                    break;
                case GroupChat groupChat:
                    break;
                case ChannelChat channelChat:
                    break;
            }
            chat.ChatId = chatId;

            chat = _context.Chats.Add(chat).Entity;
            _context.SaveChanges();
            return Result<Chat>.Success(chat);
        }
        public ChatService(AppDBContext context, ClientConnection connection)
        {
            _connection = connection;
            _context = context;
        }
        private void StartUpdatePolling()
        {
            _pollingCts?.Cancel();
            _pollingCts = new CancellationTokenSource();
            //Task.Run(() => StartUpdatePollingCycle(_pollingCts.Token, TimeSpan.FromSeconds(20)));
        }
        private void StartUpdatePollingCycle(CancellationToken token, TimeSpan delay)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    Task.Delay(delay, token).Wait();
                    var changes = _connection.Update();
                    //обработка списка изменений
                    //MessageBox.Show("hello");
                }
                catch
                {

                }
            }
        }
        private void StopUpdatePolling()
        {
            _pollingCts?.Cancel();
        }
        private void UploadChatsFromConnection(List<Guid> chats)
        {
            foreach (Guid chatId in chats)
            {
                try
                {
                    if (_context.Chats.FirstOrDefault(x => x.ChatId == chatId) != null)
                        continue;
                    var chat = _connection.LoadChat(chatId).Result;
                    _context.Chats.Add(chat);
                }
                catch
                {
                }
            }
        }
    }
}
