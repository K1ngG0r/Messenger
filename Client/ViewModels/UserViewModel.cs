using Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public class UserViewModel : ViewModel
    {
        public User User;
        public string Name => User.Name;
        public string Username => User.Username;
        public AvatarImageViewModel Avatar => new AvatarImageViewModel(User.ImagePath);
        public UserViewModel(User user)
        {
            User = user;
        }
    }
}
