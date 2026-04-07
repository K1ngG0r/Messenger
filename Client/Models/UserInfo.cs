using Client.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class UserInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string ImagePath { get; set; } = null!;
        public UserInfo(string name, string username, 
            string imagePath = "")
        {
            Name = name;
            ImagePath = imagePath;
            Username = username;
        }
        public UserInfo()
        {

        }
    }
    public class CurrentUserInfo
    {
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string ImagePath { get; set; } = null!;
        //другие личные настройки
        public CurrentUserInfo(
            string username,
            string name,
            string avatarPath)
        {
            Name = name;
            Username = username;
            ImagePath = avatarPath;
        }
    }
}
