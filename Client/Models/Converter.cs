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
        public static User ConvertToUser(UserSettings settings)
        {
            return new User(settings.name,
                settings.username,
                CacheManager.SetUserAvatarPathByUsername(settings.username,
                settings.avatar));
        }
    }
}
