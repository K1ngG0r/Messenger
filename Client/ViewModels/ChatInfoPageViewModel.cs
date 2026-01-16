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
        private ChatViewModel _chat;
        private Mediator _mediator;
        private CurrentUserService _userService;
        private Chat chatModel;
        public event Action ChatInfoClosed = null!;
        public Command ChatInfoCloseCommand { get; }
        public ChatViewModel Chat
        {
            get => _chat;
            set
            {
                _chat = value;
                OnPropertyChanged();
            }
        }
        public AvatarImageViewModel Avatar =>
            new AvatarImageViewModel(chatModel.ChatImagePath);
        public string Name =>
            Chat.ChatName;
        public string ChatType =>
            ((chatModel is PrivateChat) ? "Private chat" :
            (chatModel is GroupChat) ? "Group chat" :
            (chatModel is ChannelChat) ? "Channel chat" : string.Empty);
        public void Update(Chat chat)
        {
            chatModel = chat;
            switch (chatModel)
            {
                case PrivateChat privateChat:
                    Chat = new PrivateChatViewModel(privateChat, _mediator);
                    break;
                case GroupChat groupChat:
                    Chat = new GroupChatViewModel(groupChat, _mediator, _userService);
                    break;
                case ChannelChat channelChat:
                    Chat = new ChannelChatViewModel(channelChat, _mediator, _userService);
                    break;
                default:
                    Chat = new ChatViewModel(chatModel, _mediator);
                    break;
            }
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(ChatType));
            OnPropertyChanged(nameof(Avatar));
        }
        public ChatInfoPageViewModel(Chat chat, Mediator mediator, CurrentUserService userService)
        {
            _userService = userService;
            chatModel = chat;
            _mediator = mediator;
            switch (chatModel)
            {
                case PrivateChat privateChat:
                    _chat = new PrivateChatViewModel(privateChat, _mediator);
                    break;
                case GroupChat groupChat:
                    _chat = new GroupChatViewModel(groupChat, _mediator, _userService);
                    break;
                case ChannelChat channelChat:
                    _chat = new ChannelChatViewModel(channelChat, _mediator, _userService);
                    break;
                default:
                    _chat = new ChatViewModel(chatModel, _mediator);
                    break;
            }
            ChatInfoCloseCommand = new Command(
                () => ChatInfoClosed?.Invoke());
        }
    }
}
