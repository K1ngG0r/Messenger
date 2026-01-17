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
        protected Mediator _mediator;
        public Chat Chat { get; set; }
        public string ChatName => Chat.ChatName;
        public AvatarImageViewModel Avatar =>
            new AvatarImageViewModel(Chat.ChatImagePath);
        public string? LastMessage => Chat.Messages.LastOrDefault()?.Message;
        public ChatViewModel(Chat info, Mediator mediator)
        {
            Chat = info;
            _mediator = mediator;
            ClearHistoryCommand = new Command(OnClearHistory);
        }
        public Command ClearHistoryCommand { get; set; }
        private void OnClearHistory()
        {
            _mediator.Send(new ChatHistoryClearRequestedMessage(Chat.Id));
        }
    }
    public class PrivateChatViewModel : ChatViewModel
    {
        public UserViewModel User { get; set; }
        public Command DeleteChatCommand { get; set; }
        public PrivateChatViewModel(PrivateChat chat, Mediator mediator)
            :base(chat, mediator)
        {
            User = new UserViewModel(chat.Correspondent);
            DeleteChatCommand = new Command(OnDeleteChat);
        }
        private void OnDeleteChat()
        {
            _mediator.Send(new ChatDeletionRequestedMessage(Chat.Id));
        }
    }
    public class GroupChatViewModel : ChatViewModel
    {
        private CurrentUserService _userService;
        public bool CanExecuteOwnerAction { get; private set; }
        public bool CanExecuteAdminAction { get; private set; }
        public Command LeaveChatCommand { get; set; }
        //public Command AddParticipantCommand { get; set; }
        public RelayCommand ParticipantChatWithCommand { get; set; }
        public List<ParticipantViewModel> Participants { get; set; }
        public GroupChatViewModel(GroupChat chat, Mediator mediator, CurrentUserService userService)
            : base(chat, mediator)
        {
            _userService = userService;
            CanExecuteAdminAction = (chat.Participants.Find(x =>
            x.User.Username == userService.CurrentUser.Username &&
            x.ParticipantType is ParticipantType.Admin) != null) ||
            chat.Owner.Username == userService.CurrentUser.Username;
            ParticipantChatWithCommand = new RelayCommand(OnParticipantChatWithRequested);
            CanExecuteOwnerAction = userService.CurrentUser.Username == chat.Owner.Username;
            LeaveChatCommand = new Command(OnLeaveChat);
            Participants = chat.Participants
                .Select(x => new ParticipantViewModel(x))
                .ToList();
            Participants.Add(new ParticipantViewModel(
                new Participant(chat.Owner, chat), true));
            _mediator = mediator;
        }
        private void OnLeaveChat()
        {
            _mediator.Send(new LeaveChatMessage(Chat.Id));
        }
        private void OnParticipantChatWithRequested(object? participant)
        {
            _mediator.Send(new OpenPrivateChatMessage(((ParticipantViewModel)participant).User.Username));
        }
    }
    public class ChannelChatViewModel : ChatViewModel
    {
        private CurrentUserService _userService;
        private ParticipantViewModel? selectedParticipant;
        public UserViewModel Owner;
        public bool CanExecuteAdminAction
        {
            get
            {
                if (Owner.User.Username == _userService.CurrentUser.Username)
                    return true;
                foreach (var pt in Subscribers)
                {
                    if (pt.User.User.Username != _userService.CurrentUser.Username)
                        continue;

                    if (pt.ParticipantType is ParticipantViewModelType.Admin)
                        return true;
                }
                return false;
            }
        }
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
        public ChannelChatViewModel(ChannelChat chat, Mediator mediator, CurrentUserService userService)
            : base(chat, mediator)
        {
            _userService = userService;
            Owner = new UserViewModel(chat.Owner);
            Subscribers = chat.Subscribers
                .Select(x => new ParticipantViewModel(x))
                .ToList();
            _mediator = mediator;
        }
        private void OnUserSelected(string username)
        {
            _mediator.Send(new OpenPrivateChatMessage(username));
        }
    }
}
