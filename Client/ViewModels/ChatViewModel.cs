using Client.Models;
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
        public List<ParticipantViewModel> Participants { get; set; }
        public GroupChatViewModel(GroupChat chat)
            : base(chat)
        {
            Participants = chat.Participants
                .Select(x => new ParticipantViewModel(x))
                .ToList();
            Participants.Add(new ParticipantViewModel(
                new Participant(chat.Owner, chat), true));
        }
    }
    public class ChannelChatViewModel : ChatViewModel
    {
        public UserViewModel Owner;
        public List<ParticipantViewModel> Subscribers { get; set; }
        public ChannelChatViewModel(ChannelChat chat)
            : base(chat)
        {
            Owner = new UserViewModel(chat.Owner);
            Subscribers = chat.Subscribers
                .Select(x => new ParticipantViewModel(x))
                .ToList();
        }
    }
}
