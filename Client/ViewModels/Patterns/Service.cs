using Client.Data;
using Client.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.ViewModels.Patterns
{
    public class ChatService
    {
        private AppDBContext _context;
        public async Task<Chat> LoadChatAsync(int chatId)
        {
            return await _context.Chats
                .Include(x=>x.Messages)
                    .ThenInclude(x=>x.Who)
                .FirstAsync(x => x.Id == chatId);
        }
        public async Task<List<Chat>> LoadChatsListAsync()
        {
            return Task.Run(()=>_context.Chats
                .ToListAsync()).Result;
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
        public async Task SendMessage(ChatMessage message)
        {
            var chat = message.Chat;
            MessageBox.Show("service: сообщение отправлено");
            //отправка через udpConnection
            //сохранение сообщения в бд
        }
        public ChatService(AppDBContext context)
        { 
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
