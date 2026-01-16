using Client.Models;
using Client.ViewModels.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public class ChatViewModel:ViewModel
    {
        public Chat Chat { get; set; }
        public string ChatName => Chat.ChatName;
        public AvatarImageViewModel Avatar =>
            new AvatarImageViewModel(Chat.ChatImagePath);
        public string? LastMessage => Chat.Messages.LastOrDefault()?.Message;
        public ChatViewModel(Chat info)
        {
            Chat = info;
        }
    }
    public class PrivateChatViewModel : ChatViewModel
    {
        public UserViewModel User { get; set; }
        public PrivateChatViewModel(PrivateChat chat)
            :base(chat)
        {
            User = new UserViewModel(chat.Correspondent);
        }
    }
    public class GroupChatViewModel : ChatViewModel
    {
        private Mediator _mediator;
        private ParticipantViewModel? selectedParticipant;
        public ParticipantViewModel? SelectedParticipant
        {
            get => selectedParticipant;
            set
            {
                selectedParticipant = value;
                if (selectedParticipant != null)
                    OnUserSelected(selectedParticipant.Participant.User.Username);
            }
        }
        public List<ParticipantViewModel> Participants { get; set; }
        public GroupChatViewModel(GroupChat chat, Mediator mediator)
            : base(chat)
        {
            Participants = chat.Participants
                .Select(x => new ParticipantViewModel(x))
                .ToList();
            Participants.Add(new ParticipantViewModel(
                new Participant(chat.Owner, chat), true));
            _mediator = mediator;
        }
        private void OnUserSelected(string username)
        {
            _mediator.Send(new UserSelectedMessage(username));
        }
    }
    public class ChannelChatViewModel : ChatViewModel
    {
        public UserViewModel Owner;
        private Mediator _mediator;
        private ParticipantViewModel? selectedParticipant;
        public ParticipantViewModel? SelectedParticipant
        {
            get => selectedParticipant;
            set
            {
                selectedParticipant = value;
                if (selectedParticipant != null)
                    OnUserSelected(selectedParticipant.Participant.User.Username);
            }
        }
        public List<ParticipantViewModel> Subscribers { get; set; }
        public ChannelChatViewModel(ChannelChat chat, Mediator mediator)
            : base(chat)
        {
            Owner = new UserViewModel(chat.Owner);
            Subscribers = chat.Subscribers
                .Select(x => new ParticipantViewModel(x))
                .ToList();
            _mediator = mediator;
        }
        private void OnUserSelected(string username)
        {
            _mediator.Send(new UserSelectedMessage(username));
        }
    }
}
