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
        public static string GetChatAvatarPathByUsername(string username)
        {
            string previousPath = "C:/Users/U$er/Downloads/Messenger/Client";
            if (File.Exists(previousPath + $"/cache/avatars/{username}.png")) 
                return previousPath + $"/cache/avatars/{username}.png";
            return previousPath + $"/cache/avatars/_default.png";
        }
    }
}
