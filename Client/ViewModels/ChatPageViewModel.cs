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
        private string draftMessage = string.Empty;
        public ObservableCollection<ChatMessageViewModel> Messages { get; set; } = new();
        public string ChatName
        {
            get => chat?.ChatName ?? string.Empty;
        }
        public string DraftMessage
        {
            get => draftMessage;
            set
            {
                draftMessage = value;
                OnPropertyChanged();
            }
        }
        public Command SendMessageCommand { get; set; }
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
            SendMessageCommand = new Command(OnSendMessage);
            mediator.Register<ChatSelectedMessage>(HandleChatSelectedMessage);
        }
        private void OnSendMessage()
        {
            if (DraftMessage == string.Empty)
                return;
            if (chat is null)
                return;
            var message = new ChatMessage(chat, new User("",""),
                DraftMessage, DateTime.Now);
            Messages.Add(new ChatMessageViewModel(message));
            mediator.Send(new SendNewMessageMessage(message));
            DraftMessage = string.Empty;
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
