using System.IO;
using System.Windows;
using Client.Connection;
using Client.Data;
using Client.Models;
using Microsoft.EntityFrameworkCore;

namespace Client.ViewModels.Patterns
{
    public class ChatService
    {
        private ClientConnection _connection;
        private AppDBContext _context;
        public UserSettings CurrentUserSettings;
        public User CurrentUser
            => UserConverter
            .ConvertToUser(CurrentUserSettings);
        public void OnLogout()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }
        public void DeleteMessage(int messageId)
        {
            //сервер не должен пока об этом знать
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
            //сервер должен об этом знать
            _context.Remove(_context.Chats.First(x => x.Id == chatId));
            _context.SaveChangesAsync();
        }
        public User? TryLoadUserByUsername(string username)
        {
            var user = _context.Users.FirstOrDefault(x => x.Username == username);
            if (user is null)
            {
                try
                {
                    user = _connection.LoadUser(username).Result;
                }
                catch
                {
                    user = null;
                }
            }
            return user;
        }
        public Chat? TryLoadPrivateChatByUsername(string username)
        {
            var chat = _context.Chats
                .OfType<PrivateChat>()
                .Include(x => x.Messages)
                    .ThenInclude(x => x.Who)
                .FirstOrDefault(x => x.Correspondent.Username == username);
            return chat;
        }
        public bool TryLogin(string username, string password)
        {
            try
            {
                var settings = _connection
                    .Login(username, password).Result;
                CurrentUserSettings = settings.Item1;
                UploadChatsFromConnection(settings.Item2);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public Chat? TryLoadChat(int chatId)
        {
            var groupChat = _context.Chats.OfType<GroupChat>()
                .Include(x=>x.Owner)
                .Include(x => x.Messages).ThenInclude(x => x.Who)
                .Include(x => x.Participants).ThenInclude(x => x.User)
                .FirstOrDefault(x => x.Id == chatId);
            if (groupChat != null) return groupChat;
            var channelChat = _context.Chats.OfType<ChannelChat>()
                .Include(x => x.Messages).ThenInclude(x => x.Who)
                .Include(x => x.Subscribers).ThenInclude(x => x.User)
                .FirstOrDefault(x => x.Id == chatId);
            if (channelChat != null) return channelChat;
            var privateChat = _context.Chats.OfType<PrivateChat>()
                .Include(x=>x.Correspondent)
                .Include(x => x.Messages).ThenInclude(x => x.Who)
                .FirstOrDefault(x => x.Id == chatId);
            if (privateChat != null) return privateChat;
            return null;
        }
        public List<Chat> LoadChatsList()
        {
            return _context.Chats.ToList();
        }
        public ChatMessage SendMessage(ChatMessage message)
        {
            try
            {
                _connection.SendMessage(message.Chat.ChatId, message.Message).Wait();
                message.State = ChatMessage.ChatMessageState.Delivered;
            }
            catch
            {
                message.State = ChatMessage.ChatMessageState.NotDelivered;
            }
            var result = _context.Messages.Add(message).Entity;
            _context.SaveChanges();
            return result;
        }
        public Chat? CreateNewChat(Chat chat)
        {
            try
            {
                Guid chatId = default;
                switch (chat)
                {
                    case PrivateChat privateChat:
                        chatId = _connection.CreatePrivateChat(
                            privateChat.Correspondent.Username).Result;
                        break;
                    case GroupChat groupChat:
                        break;
                    case ChannelChat channelChat:
                        break;
                }
                chat.ChatId = chatId;
            }
            catch
            {
                return null;
            }

            var result = _context.Chats.Add(chat).Entity;
            _context.SaveChanges();
            return result;
        }
        public ChatService(AppDBContext context, ClientConnection connection)
        {
            _connection = connection;
            _context = context;
        }
        private void UploadChatsFromConnection(List<Guid> chats)
        {
            foreach (Guid chatId in chats)
            {
                if (_context.Chats.FirstOrDefault(x => x.ChatId == chatId) != null)
                    continue;
                try
                {
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
