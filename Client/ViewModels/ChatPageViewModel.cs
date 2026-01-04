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

namespace Client.ViewModels
{
    public class ChatPageViewModel : ViewModel
    {
        private ChatService _chatService;
        private CurrentUserService _userService;
        private Mediator _mediator;
        private Chat? chat;
        private string draftMessage = string.Empty;
        private ObservableCollection<ChatMessageViewModel> messages = new();
        public ObservableCollection<ChatMessageViewModel> Messages
        {
            get => messages;
            set
            {
                messages = value;
                OnPropertyChanged();
            }
        }
        public SolidColorBrush? BackgroundChatImage => (chat is null) ? null : new SolidColorBrush(Colors.Gray);
        public string ChatName => chat?.ChatName ?? string.Empty;
        public AvatarImageViewModel Avatar
        {
            get => new AvatarImageViewModel(chat?.ChatImagePath ?? string.Empty);
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
        public async Task UpdateChat(int chatId)
        {
            chat = await _chatService.LoadChatAsync(chatId);
            Messages = new ObservableCollection<ChatMessageViewModel>(
                chat.Messages.Select(x => RegisterChatMessageViewModel(new ChatMessageViewModel(x, _userService))));
            OnPropertyChanged(nameof(ChatName));
            OnPropertyChanged(nameof(Avatar));
            OnPropertyChanged(nameof(BackgroundChatImage));
        }
        public ChatPageViewModel(Mediator messenger, ChatService chatService, CurrentUserService userService)
        {
            _chatService = chatService;
            _userService = userService;
            _mediator = messenger;
            SendMessageCommand = new Command(OnSendMessage);
            _mediator.Register<ChatSelectedMessage>(HandleChatSelectedMessage);
        }
        private async void OnSendMessage()
        {
            if (DraftMessage == string.Empty)
                return;
            if (chat is null)
                return;

            var message = await _chatService.SendMessageAsync(
                new ChatMessage(chat, _userService.CurrentUser,
                    DraftMessage, DateTime.Now));

            Messages.Add(RegisterChatMessageViewModel(new ChatMessageViewModel(message, _userService)));
            DraftMessage = string.Empty;
        }
        private ChatMessageViewModel RegisterChatMessageViewModel(ChatMessageViewModel vm)
        {
            vm.DeletionRequested += OnChatMessageDeletionRequested;
            return vm;
        }
        private void OnChatMessageDeletionRequested(int messageId)
        {
            Messages.Remove(Messages.First(x => x.ChatMessage.Id == messageId));
            _chatService.DeleteMessage(messageId);
        }
        private async void HandleChatSelectedMessage(object? newChatObject)
        {
            ChatSelectedMessage? chatSelectedMessage = (ChatSelectedMessage?)newChatObject;
            if (chatSelectedMessage is null)
                return;
            await UpdateChat(chatSelectedMessage.ChatId);
        }
    }
}
