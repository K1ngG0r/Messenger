using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Models;
using Client.ViewModels.Patterns;

namespace Client.ViewModels
{
    public class MainPageViewModel : ViewModel
    {
        private Mediator mediator;
        private ChatViewModel? selectedContact;
        public ObservableCollection<ChatViewModel> ChatsList { get; set; } = new();
        public string SearchText { get; set; } = string.Empty;
        public void UpdateChatsList(List<Chat> chats)
        {
            ChatsList = new ObservableCollection<ChatViewModel>(
                chats.Select(x=>new ChatViewModel(x)));
        }
        public ChatViewModel? SelectedChat
        {
            get => selectedContact;
            set
            {
                selectedContact = value;
                OnPropertyChanged();
                OnChatSelected(value);
            }
        }
        public MainPageViewModel(Mediator messenger)
        {
            mediator = messenger;
        }
        private void OnChatSelected(ChatViewModel? chat)
        {
            if (chat is null)
                return;
            mediator.Send(new ChatSelectedMessage(chat.Chat));
        }
    }
}
