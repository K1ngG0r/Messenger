using Client.Connection;
using Client.Connection.Pattenrs;
using Client.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Patterns.Services
{
    public class AuthorizationService
    {
        private ClientConnection _connection;
        public AuthorizationService(ClientConnection connection)
        {
            _connection = connection;
        }

        public async Task<Result<CurrentUserInfo>> TryLogin(string username, string password)
        {
            var settings = await _connection
                .Login(username, password);
            CurrentUserSettings = UserConverter
                .ConvertUserSettingsToCurrentUserSettings(settings.Item1);
            UploadChatsFromConnection(settings.Item2);
            return true;
        }
    }
}
