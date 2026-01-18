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
        public static string SetChatAvatarByChatId(int chatId, byte[] avatarBytes)
        {
            return GetChatAvatarByChatId(chatId);//fixit
        }
        public static string SetUserAvatarPathByUsername(string username, byte[] avatarBytes)
        {
            return GetUserAvatarPathByUsername(username);//fixit
        }
    }
}
