using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public static class AvatarsManager
    {
        public static string GetUserAvatarPathByUsername(string username)
        {
            string previousPath = "C:/Users/U$er/Downloads/Messenger/Client";
            if (File.Exists(previousPath + $"/cache/avatars/users/{username}.png")) 
                return previousPath + $"/cache/avatars/users/{username}.png";
            return previousPath + $"/cache/avatars/_default.png";
        }
        public static string GetChatAvatarByChatId(int chatId)
        {
            string previousPath = "C:/Users/U$er/Downloads/Messenger/Client";
            if (File.Exists(previousPath + $"/cache/avatars/chats/id_{chatId}.png"))
                return previousPath + $"/cache/avatars/chats/id_{chatId}.png";
            return previousPath + $"/cache/avatars/_default.png";
        }
        public static void SetChatAvatarByChatId(int chatId, byte[] avatarBytes)
        {
            //можно обновить или сохранить аватарку для чата
            //fixit
        }
        public static void SetUserAvatarPathByUsername(int username, byte[] avatarBytes)
        {
            //можно обновить или сохранить аватарку для пользователя
            //fixit
        }
    }
}
