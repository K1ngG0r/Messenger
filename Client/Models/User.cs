using Client.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string ImagePath { get; set; } = null!;
        public User(string name, string username, 
            string imagePath = "")
        {
            Name = name;
            ImagePath = imagePath;
            Username = username;
        }
        public User()
        {

        }
    }
}
