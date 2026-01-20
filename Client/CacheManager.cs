using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public static class CacheManager
    {
        private static string previousPath = "C:/Users/U$er/Downloads/Messenger/Client/cache/";
        public static string? TryGetPreviousUserSessionKey()
        {
            try
            {
                using (FileStream fs = new FileStream(previousPath + "PreviousUsername.txt", FileMode.Open))
                using (StreamReader sr = new StreamReader(fs))
                {
                    return sr.ReadLine();
                }
            }
            catch
            {
                return null;
            }
        }
        public static void ClearPreviousSessionKey()
        {
            SetPreviousSessionKey(string.Empty);
        }
        public static void SetPreviousSessionKey(string sessionKey)
        {
            using (FileStream fs = new FileStream(previousPath + "PreviousUsername.txt", FileMode.OpenOrCreate))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(sessionKey);
            }
        }
        public static string GetUserAvatarPathByUsername(string username)
        {
            if (File.Exists(previousPath + $"avatars/users/{username}.png")) 
                return previousPath + $"avatars/users/{username}.png";
            return previousPath + $"avatars/_default.png";
        }
        public static string GetChatAvatarByChatId(int chatId)
        {
            if (File.Exists(previousPath + $"avatars/chats/id_{chatId}.png"))
                return previousPath + $"avatars/chats/id_{chatId}.png";
            return previousPath + $"avatars/_default.png";
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
