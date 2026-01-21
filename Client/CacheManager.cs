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
        private static string previousPath = "C:/Users/U$er/Downloads/m/Messenger/Client/cache/";
        public static (string, string)? TryGetPreviousLoginSettings()
        {
            try
            {
                using (FileStream fs = new FileStream(previousPath + "PreviousUsername.txt", FileMode.Open))
                using (StreamReader sr = new StreamReader(fs))
                {
                    var line = sr.ReadLine();
                    if (line is null)
                        return null;
                    var splittedLine = line.Split();
                    if (splittedLine.Length == 2)
                        return (splittedLine[0], splittedLine[1]);
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        public static void ClearPreviousLoginSettings()
        {
            var filePath = previousPath + "PreviousUsername.txt";
            if(File.Exists(filePath))
                File.Delete(filePath);
            File.Create(filePath);
        }
        public static void SetNewLoginSettings(string login, string password)
        {
            var filePath = previousPath + "PreviousUsername.txt";
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(login + " " + password);
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
