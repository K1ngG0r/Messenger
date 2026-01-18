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
        public Command CreatePrivateChatCommand { get; set; }
        public Command CreateGroupChatCommand { get; set; }
        public Command CreateChannelChatCommand { get; set; }
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
            CreatePrivateChatCommand = new Command(OnCreatePrivateChat);
            CreateGroupChatCommand = new Command(OnCreateGroupChat);
            CreateChannelChatCommand = new Command(OnCreateChannelChat);
            _chatService = chatService;
            _mediator = messenger;
            NavigateToSettingsPageCommand = new Command(NavigateToSettingsPage);
            _mediator.Register<ChatDeletionRequestedMessage>(HandleChatDeletionRequestedMessage);
            _mediator.Register<ChatCreatedMessage>(HandleChatCreatedMessage);
            _mediator.Register<LeaveChatMessage>(HandleLeaveChatMessage);
            LoadChats();
        }
        private void NavigateToSettingsPage()
        {
            _mediator.Send(new NavigateToSettingsPageMessage());
        }
        private void LoadChats()
        {
            ChatsList = new ObservableCollection<ChatViewModel>(
                _chatService.LoadChatsList()
                    .Select(x=>new ChatViewModel(x, _mediator)));
        }
        private void OnChatSelected(Chat? chat)
        {
            if (chat is null)
                return;
            _mediator.Send(new ChatSelectedMessage(chat.Id));
        }
        private void HandleChatDeletionRequestedMessage(object? obj)
        {
            ChatDeletionRequestedMessage message = (ChatDeletionRequestedMessage)obj;
            var chat = ChatsList.FirstOrDefault(x => x.Chat.Id == message.ChatId);
            if (chat is null)
                return;
            ChatsList.Remove(chat);
        }
        private void HandleLeaveChatMessage(object? obj)
        {
            LeaveChatMessage message = (LeaveChatMessage)obj;
            var chat = ChatsList.FirstOrDefault(x => x.Chat.Id == message.ChatId);
            if (chat is null)
                return;
            ChatsList.Remove(chat);
        }
        private void HandleChatCreatedMessage(object? obj)
        {
            ChatCreatedMessage message = (ChatCreatedMessage)obj;
            var chat = _chatService.TryLoadChat(message.ChatId);
            if (chat is null)
                return;
            ChatsList.Add(new ChatViewModel(chat, _mediator));
        }
        private void OnCreatePrivateChat()
        {
            _mediator.Send(new PrivateChatCreationRequestedMessage());
        }
        private void OnCreateGroupChat()
        {

        }
        private void OnCreateChannelChat()
        {

        }

    }
}
