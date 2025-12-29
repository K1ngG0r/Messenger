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
        public string ChatName => chat?.ChatName ?? string.Empty;
        public string ImagePath => chat?.ChatImagePath ?? string.Empty;
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
                chat.Messages.Select(x => new ChatMessageViewModel(x, _userService)));
            OnPropertyChanged(nameof(ChatName));
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
            var message = new ChatMessage(chat, _userService.CurrentUser,
                DraftMessage, DateTime.Now);
            Messages.Add(new ChatMessageViewModel(message, _userService));
            await _chatService.SendMessageAsync(chat, message);
            DraftMessage = string.Empty;
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
