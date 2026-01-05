using Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Client.ViewModels
{
    public class ParticipantViewModel:ViewModel
    {
        public Participant Participant;
        public UserViewModel User => new UserViewModel(Participant.User);
        public string ParticipantType => Participant.ParticipantType.ToString();
        public ParticipantViewModel(Participant participant)
        {
            Participant = participant;
        }
    }
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
