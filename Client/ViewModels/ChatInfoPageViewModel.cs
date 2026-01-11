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
        private Chat chatModel;
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
                    Chat = new PrivateChatViewModel(privateChat);
                    break;
                case GroupChat groupChat:
                    Chat = new GroupChatViewModel(groupChat);
                    break;
                case ChannelChat channelChat:
                    Chat = new ChannelChatViewModel(channelChat);
                    break;
                default:
                    Chat = new ChatViewModel(chatModel);
                    break;
            }
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(ChatType));
            OnPropertyChanged(nameof(Avatar));
        }
        public ChatInfoPageViewModel(Chat chat)
        {
            chatModel = chat;
            switch (chatModel)
            {
                case PrivateChat privateChat:
                    _chat = new PrivateChatViewModel(privateChat);
                    break;
                case GroupChat groupChat:
                    _chat = new GroupChatViewModel(groupChat);
                    break;
                case ChannelChat channelChat:
                    _chat = new ChannelChatViewModel(channelChat);
                    break;
                default:
                    _chat = new ChatViewModel(chatModel);
                    break;
            }
        }
    }
}
