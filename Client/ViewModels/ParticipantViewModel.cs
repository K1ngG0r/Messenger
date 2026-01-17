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
        }
    }
    public enum ParticipantViewModelType
    {
        Owner,
        Admin,
        Member
    }
}
