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
        public static User ConvertCurrentUserSettingsToUser(CurrentUserSettings settings)
        {
            return new User(settings.Name,
                settings.Username,
                CacheManager.GetUserAvatarPathByUsername(settings.Username));
        }
        public static User ConvertUserSettingsToUser(UserSettings settings)
        {
            return new User(settings.name,
                settings.username,
                CacheManager.SetUserAvatarPathByUsername(settings.username, settings.avatar));
        }
        public static CurrentUserSettings ConvertUserSettingsToCurrentUserSettings(UserSettings settings)
        {
            return new CurrentUserSettings(settings.name,
                settings.username,
                CacheManager.SetUserAvatarPathByUsername(settings.username, settings.avatar));
        }
    }
}
