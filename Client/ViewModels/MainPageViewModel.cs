using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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
        public Command NavigateToSettingsPageCommand { get; set; }
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
                OnChatSelected(value?.Chat);
            }
        }
        public MainPageViewModel(Mediator messenger, ChatService chatService)
        {
            _chatService = chatService;
            _mediator = messenger;
            NavigateToSettingsPageCommand = new Command(NavigateToSettingsPage);
            LoadChats();
        }
        private void NavigateToSettingsPage()
        {
            _mediator.Send(new NavigateToSettingsPage());
        }
        private void LoadChats()
        {
            ChatsList = new ObservableCollection<ChatViewModel>(
                _chatService.LoadChatsList()
                    .Select(x=>new ChatViewModel(x)));
        }
        private void OnChatSelected(Chat? chat)
        {
            if (chat is null)
                return;
            _mediator.Send(new ChatSelectedMessage(chat.Id));
        }
    }
}
