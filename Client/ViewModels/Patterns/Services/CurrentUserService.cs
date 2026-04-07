using Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Patterns.Services
{
    public class CurrentUserService
    {
        public CurrentUserInfo UserInfo { get; set; }
        public CurrentUserService(CurrentUserInfo? userInfo) 
        {
            UserInfo = userInfo is null ? 
                new CurrentUserInfo(string.Empty, string.Empty, string.Empty) : userInfo;
        }
    }
}
