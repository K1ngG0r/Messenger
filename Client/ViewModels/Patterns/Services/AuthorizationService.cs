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

        public async Task<Result<LoginResponseSettings>> TryLogin(string username, string password)
        {
            var settings = await _connection
                .Login(username, password);
            if (settings.IsFailed)
                return Result<LoginResponseSettings>.Failure();
            return Result<LoginResponseSettings>.Success(settings.Value!);
        }
    }
}
