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
            return _context.Chats.ToList();

            /*var cts = new CancellationTokenSource();
            var task = Task.Run(() => _context.Chats.ToListAsync());
            Task.Delay(5000).Wait();
            cts.Cancel();
            if (task.IsCompleted)
            {
                return task.Result;
            }
            return new List<Chat>();*/
        }
        public async Task SendMessage(Chat to,ChatMessage message)
        {
            var chat = message.Chat;
            _connection.SendMessage(chat, message);
            _context.Messages.Add(message);//доработать
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
