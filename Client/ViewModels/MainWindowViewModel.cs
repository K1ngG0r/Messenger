using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Client.Models;
using Client.ViewModels.Patterns;
using Client.Views;

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
        public CreatePrivateChatPageViewModel CreatePrivateChatPageViewModel { get; }
        public MainPageViewModel MainPageViewModel { get; }
        public SettingsPageViewModel SettingsPageViewModel { get; }
        public ChatPageViewModel? ChatPageViewModel { get; private set; }
        public MainWindowViewModel(Mediator mediator, ChatService chatService, CurrentUserService userService)
        {
            _mediator = mediator;
            _chatService = chatService;
            _userService = userService;
            CreatePrivateChatPageViewModel = new CreatePrivateChatPageViewModel(mediator, chatService);
            MainPageViewModel = new MainPageViewModel(mediator, chatService);
            ChatPageViewModel = null;
            SettingsPageViewModel = new SettingsPageViewModel(mediator, userService);
            activePageViewModel = MainPageViewModel;
            _mediator.Register<NavigateToSettingsPageMessage>(HandleNavigateToSettingsPage);
            _mediator.Register<NavigateToMainPageMessage>(HandleNavigateToMainPage);
            _mediator.Register<ChatSelectedMessage>(HandleChatSelectedMessage);
            _mediator.Register<OpenPrivateChatMessage>(HandleOpenPrivateChatMessage);
            _mediator.Register<ChatDeletionRequestedMessage>(HandleChatDeletionRequestedMessage);
            _mediator.Register<LeaveChatMessage>(HandleLeaveChatMessage);
            _mediator.Register<PrivateChatCreationRequestedMessage>
                (HandlePrivateChatCreationRequestedMessage);
        }
        private void HandleNavigateToSettingsPage(object? parameters)
        {
            ActivePageViewModel = SettingsPageViewModel;
        }
        private void HandleNavigateToMainPage(object? parameters)
        {
            ActivePageViewModel = MainPageViewModel;
        }
        private async void HandleChatSelectedMessage(object? newChatObject)
        {
            ChatSelectedMessage? message = (ChatSelectedMessage?)newChatObject;
            if (message is null)
                return;
            if (ChatPageViewModel is null)
                ChatPageViewModel = new ChatPageViewModel(message.ChatId, _mediator, _chatService, _userService);
            else
                await ChatPageViewModel.UpdateChat(message.ChatId);
            OnPropertyChanged(nameof(ChatPageViewModel));
        }
        private void HandleLeaveChatMessage(object? newUserObject)
        {
            LeaveChatMessage? message = (LeaveChatMessage?)newUserObject;
            if (message is null)
                return;
            _chatService.DeleteChat(message.ChatId);
            if (ChatPageViewModel is null)
                return;
            if (ChatPageViewModel.Chat.Id != message.ChatId)
                return;
            ChatPageViewModel = null;
            OnPropertyChanged(nameof(ChatPageViewModel));
        }
        private async void HandleOpenPrivateChatMessage(object? newUserObject)
        {
            OpenPrivateChatMessage? message = (OpenPrivateChatMessage?)newUserObject;
            if (message is null)
                return;
            if (_chatService.TryLoadUserByUsername(message.Username) is null)
                return;
            if (ChatPageViewModel is null)
                ChatPageViewModel = new ChatPageViewModel(message.Username, _mediator, _chatService, _userService);
            else
                await ChatPageViewModel.UpdateChatByUsername(message.Username);
            OnPropertyChanged(nameof(ChatPageViewModel));
        }
        private void HandleChatDeletionRequestedMessage(object? newChatObject)
        {
            ChatDeletionRequestedMessage? message = (ChatDeletionRequestedMessage?)newChatObject;
            if (message is null)
                return;
            _chatService.DeleteChat(message.ChatId);
            if (ChatPageViewModel is null)
                return;
            if (ChatPageViewModel.Chat.Id != message.ChatId)
                return;
            ChatPageViewModel = null;
            OnPropertyChanged(nameof(ChatPageViewModel));
        }
        private void HandlePrivateChatCreationRequestedMessage(object? newChatObject)
        {
            ActivePageViewModel = CreatePrivateChatPageViewModel;
        }
    }
}
