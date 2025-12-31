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
        private Mediator _mediator;

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
        public ChatService(AppDBContext context, ClientConnection connection, Mediator mediator)
        {
            _connection = connection;
            _context = context;
            _connection.NewResponse += HandleNewResponse;
            _mediator = mediator;
        }
        private void HandleNewResponse(Response response)
        {

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
