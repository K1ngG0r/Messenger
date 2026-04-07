using Client.Connection;
using Client.Connection.Pattenrs;
using Client.Data;
using Client.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Patterns.Services
{
    public class LoadService
    {
        private ClientConnection _connection;
        private AppDBContext _context;
        public LoadService(AppDBContext context, ClientConnection conntection)
        {
            _context = context;
            _connection = conntection;
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
    }
}
