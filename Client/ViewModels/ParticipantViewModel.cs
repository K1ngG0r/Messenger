using Client.Models;
using Client.ViewModels.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Client.ViewModels
{
    public class ParticipantViewModel : ViewModel
    {
        public Participant Participant { get; set; }
        public UserViewModel User { get; set; }
        public ParticipantViewModelType ParticipantType { get; set; }
        public Command ChatWithCommand { get; set; }
        public event Action<string> ChatWithRequested = null!;
        public bool CanExecuteAdminAction => !(ParticipantType is ParticipantViewModelType.Member);
        public ParticipantViewModel(Participant participant, bool isOwner = false)
        {
            Participant = participant;
            User = new UserViewModel(participant.User);
            if (isOwner)
                ParticipantType = ParticipantViewModelType.Owner;
            else
            {
                ParticipantType = (participant.ParticipantType
                    is Models.ParticipantType.Member)
                    ? ParticipantViewModelType.Member :
                    ParticipantViewModelType.Admin;
            }
            ChatWithCommand = new Command(OnChatWithRequested);
        }
        private void OnChatWithRequested()
        {
            ChatWithRequested?.Invoke(User.User.Username);
        }
    }
    public enum ParticipantViewModelType
    {
        Owner,
        Admin,
        Member
    }
}
