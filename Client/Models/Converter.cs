using Client.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public static class UserConverter
    {
        public static UserInfo ConvertCurrentUserSettingsToUser(CurrentUserInfo settings)
        {
            return new UserInfo(settings.Name,
                settings.Username,
                CacheManager.GetUserAvatarPathByUsername(settings.Username));
        }
        public static UserInfo ConvertUserSettingsToUser(UserSettings settings)
        {
            return new UserInfo(settings.name,
                settings.username,
                CacheManager.SetUserAvatarPathByUsername(settings.username, settings.avatar));
        }
    }
}
