using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Client.Models;
using Client.ViewModels.Patterns;

namespace Client.ViewModels
{
    public class ChatPageViewModel : ViewModel
    {
        private Mediator mediator;
        private Chat? chat;
        public ObservableCollection<ChatMessageViewModel> Messages { get; set; } = new();
        public string ChatName
        {
            get => chat?.ChatName ?? string.Empty;
        }

        public void UpdateChat(Chat newChat)
        {
            chat = newChat;
            Messages = new ObservableCollection<ChatMessageViewModel>(
                newChat.Messages.Select(x => new ChatMessageViewModel(x)));
            OnPropertyChanged(nameof(Messages));
            OnPropertyChanged(nameof(ChatName));
        }
        public ChatPageViewModel(Mediator messenger)
        {
            mediator = messenger;
            mediator.Register<ChatSelectedMessage>(HandleChatSelectedMessage);
        }
        private void HandleChatSelectedMessage(object? newChatObject)
        {

            ChatSelectedMessage? chatSelectedMessage = (ChatSelectedMessage?)newChatObject;
            if (chatSelectedMessage is null)
                return;
            UpdateChat(chatSelectedMessage.SelectedChat);
        }
    }
}
