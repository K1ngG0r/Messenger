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
        public async Task UploadChatsFromConnection(List<Guid> chats)
        {
            foreach (Guid chatId in chats)
            {
                if (_context.Chats.FirstOrDefault(x => x.ChatId == chatId) != null)
                    continue;
                var chat = await _connection.LoadChat(chatId);
                if (chat.IsFailed)
                {
                    continue;
                }
                _context.Chats.Add(chat.Value!);
            }
        }
    }
}
