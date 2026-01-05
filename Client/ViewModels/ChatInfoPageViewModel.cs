using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Client.Models;
using Client.ViewModels.Patterns;
using Microsoft.Extensions.Primitives;

namespace Client.ViewModels
{
    public class ChatInfoPageViewModel : ViewModel
    {
        public Chat? Chat;
        public AvatarImageViewModel Avatar => new AvatarImageViewModel(Chat?.ChatImagePath ?? string.Empty);
        public string? Name => Chat?.ChatName;
        public string? Username => (Chat as PrivateChat)?.Correspondent.Username;
        public List<ParticipantViewModel>? Participants=> ((Chat as GroupChat)?.Participants ??
                    (Chat as ChannelChat)?.Subscribers)
                    ?.Select(x => new ParticipantViewModel(x))
                    .ToList();
        public string? ChatType => (Chat is PrivateChat) ? "Private chat" :
            (Chat is GroupChat) ? "Group chat" :
            (Chat is ChannelChat) ? "Channel chat" : null;

        public ChatInfoPageViewModel(Chat? chat)
        {
            Chat = chat;
        }
        public ChatInfoPageViewModel()
        {

        }
    }
}
