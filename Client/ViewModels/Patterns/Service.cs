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
            _context.SaveChanges();
        }
        public Task<Chat> LoadChatAsync(int chatId)
        {
            return Task.Run(() => LoadChat(chatId));
        }
        public Chat LoadChat(int chatId)
        {
            var groupChat = _context.Chats.OfType<GroupChat>()
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
                .Include(x => x.Messages).ThenInclude(x => x.Who)
                .FirstOrDefault(x => x.Id == chatId);
            if (privateChat != null) return privateChat;
            throw new InvalidOperationException($"Chat with Id {chatId} not found.");
        }
        public List<Chat> LoadChatsList()
        {
            return _context.Chats.ToList();
        }
        public async Task<ChatMessage> SendMessageAsync(ChatMessage message)
        {
            /*Request sendMessageRequest = new Request();
            await _connection.SendAsync(sendMessageRequest);*/

            var result = _context.Messages.Add(message).Entity;
            await _context.SaveChangesAsync();
            return result;
        }
        public async Task<Chat> CreateNewChat(Chat chat)
        {
            /*Request sendMessageRequest = new Request();
            await _connection.SendAsync(sendMessageRequest);*/

            var result = _context.Chats.Add(chat).Entity;
            await _context.SaveChangesAsync();
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
