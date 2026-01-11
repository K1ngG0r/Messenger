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
    public class MainWindowViewModel : ViewModel
    {
        private Mediator _mediator;
        private ChatService _chatService;
        private CurrentUserService _userService;
        private ViewModel activePageViewModel;
        public ViewModel ActivePageViewModel
        {
            get => activePageViewModel;
            set
            {
                activePageViewModel = value;
                OnPropertyChanged();
            }
        }
        public MainPageViewModel MainPageViewModel { get;}
        public SettingsPageViewModel SettingsPageViewModel { get;}
        public ChatPageViewModel? ChatPageViewModel { get; private set; }
        public MainWindowViewModel(Mediator mediator, ChatService chatService, CurrentUserService userService)
        {
            _mediator = mediator;
            _chatService = chatService;
            _userService = userService;
            MainPageViewModel = new MainPageViewModel(mediator, chatService);
            ChatPageViewModel = null;
            SettingsPageViewModel = new SettingsPageViewModel(mediator, userService);
            activePageViewModel = MainPageViewModel;
            _mediator.Register<NavigateToSettingsPage>(OnNavigateToSettingsPage);
            _mediator.Register<NavigateToMainPage>(OnNavigateToMainPage);
            _mediator.Register<ChatSelectedMessage>(OnChatSelected);
        }
        private void OnNavigateToSettingsPage(object? parameters)
        {
            ActivePageViewModel = SettingsPageViewModel;
        }
        private void OnNavigateToMainPage(object? parameters)
        {
            ActivePageViewModel = MainPageViewModel;
        }
        private void OnChatSelected(object? parameters)
        {
            var chatSelectedMessage = (ChatSelectedMessage?)parameters;
            if (chatSelectedMessage is null)
                return;
            _mediator.Unregister<ChatSelectedMessage>(OnChatSelected);
            ChatPageViewModel = new ChatPageViewModel(chatSelectedMessage.ChatId, _mediator, _chatService, _userService);
            OnPropertyChanged(nameof(ChatPageViewModel));
        }
    }
}
