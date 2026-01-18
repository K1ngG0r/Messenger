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
        public User? TryLoadUserByUsername(string username)
        {
            //загрузка из сервер _connection.LoadUser(username);
            var user = _context.Users.FirstOrDefault(x => x.Username == username);
            if (user is null)
            {
                try
                {
                    user = Task.Run(()=>_connection.LoadUser(username)).Result;
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
                Task.Run(()=>_connection.SendMessage(message.Chat.ChatId, message.Message)).Wait();
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
                        chatId = Task.Run(() => _connection.CreateChat(
                            CreateChatRequestSettingsMethod.PrivateChat,
                            privateChat.Correspondent.Username)).Result;
                        break;
                    case GroupChat groupChat:
                        chatId = Task.Run(() => _connection.CreateChat(
                            CreateChatRequestSettingsMethod.GroupChat,
                            groupChat.ChatName)).Result;
                        break;
                    case ChannelChat channelChat:
                        chatId = Task.Run(() => _connection.CreateChat(
                            CreateChatRequestSettingsMethod.ChannelChat,
                            channelChat.ChatName)).Result;
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
    }
    public class CurrentUserService
    {
        public User CurrentUser;
        public CurrentUserService(User user)
        {
            CurrentUser = user;
        }
    }
}
