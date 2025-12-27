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
        public string PublicKey { get; set; } = null!;
        //аватарка
        public User(string name, string username)
        {
            Name = name;
            Username = username;
        }
        public User()
        {

        }
    }
}
