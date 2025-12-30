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
        public async Task<Chat> LoadChatAsync(int chatId)
        { 
            return await _context.Chats
                .Include(x => x.Messages)
                    .ThenInclude(x => x.Who)
                .FirstAsync(x => x.Id == chatId);
        }
        public List<Chat> LoadChatsList()
        {
            return _context.Chats.ToList();
        }
        public async Task<ChatMessage> SendMessageAsync(Chat to, ChatMessage message)
        {
            await _connection.SendMessageAsync(to, message);
            var result = _context.Messages.Add(message).Entity;
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
