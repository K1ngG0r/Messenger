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
        public async Task<Chat> LoadChatAsync(int chatId)
        {
            return await _context.Chats
                .Include(x => x.Messages)
                    .ThenInclude(x => x.Who)
                .FirstAsync(x => x.Id == chatId);
        }
        public async Task<List<Chat>> LoadChatsListAsync()
        {
            var task = Task.Run(_context.Chats.ToList);
            return task.Result;
        }
        public async Task SendMessageAsync(Chat to, ChatMessage message)
        {
            var chat = message.Chat;
            await _connection.SendMessageAsync(chat, message);
            _context.Messages.Add(message);//доработать
            await _context.SaveChangesAsync();
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
