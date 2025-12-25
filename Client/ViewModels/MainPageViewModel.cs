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
        private Mediator _mediator;
        private ChatService _chatService;
        private ChatViewModel? selectedContact;
        private ObservableCollection<ChatViewModel> chatsList = new();
        private string searchText = string.Empty;
        public ObservableCollection<ChatViewModel> ChatsList
        {
            get => chatsList;
            set
            {
                chatsList = value;
                OnPropertyChanged();
            }
        }
        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                OnPropertyChanged();
            }
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
        public MainPageViewModel(Mediator messenger, ChatService chatService)
        {
            _chatService = chatService;
            _mediator = messenger;
            LoadChats().Wait();
        }
        private async Task LoadChats()
        {
            ChatsList = new ObservableCollection<ChatViewModel>(
                (await _chatService.LoadChatsListAsync())
                    .Select(x=>new ChatViewModel(x)));
        }
        private void OnChatSelected(ChatViewModel? chat)
        {
            if (chat is null)
                return;
            _mediator.Send(new ChatSelectedMessage(chat.Chat.Id));
        }
    }
}
