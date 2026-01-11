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
        private Chat chat;
        private string draftMessage = string.Empty;
        private ObservableCollection<ChatMessageViewModel> messages;
        private ChatInfoPageViewModel chatInfo;
        public ObservableCollection<ChatMessageViewModel> Messages
        {
            get => messages;
            set
            {
                messages = value;
                OnPropertyChanged();
            }
        }
        public string ChatName => chat.ChatName;
        public AvatarImageViewModel Avatar
        {
            get => new AvatarImageViewModel(chat.ChatImagePath);
        }
        public ChatInfoPageViewModel ChatInfo
        {
            get => chatInfo;
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
            chat = _chatService.LoadChat(chatId);
            Messages = new ObservableCollection<ChatMessageViewModel>(
                chat.Messages.Select(x => RegisterChatMessageViewModel(new ChatMessageViewModel(x, _userService))));
            ChatInfo.Update(chat);
            OnPropertyChanged(nameof(ChatName));
            OnPropertyChanged(nameof(Avatar));
        }
        public ChatPageViewModel(int chatId, Mediator messenger, ChatService chatService, CurrentUserService userService)
        {
            _chatService = chatService;
            _userService = userService;
            _mediator = messenger;
            SendMessageCommand = new Command(OnSendMessage);
            _mediator.Register<ChatSelectedMessage>(HandleChatSelectedMessage);
            chat = _chatService.LoadChat(chatId);
            messages = new ObservableCollection<ChatMessageViewModel>(
                chat.Messages.Select(x => RegisterChatMessageViewModel(new ChatMessageViewModel(x, _userService))));
            chatInfo = new ChatInfoPageViewModel(chat);
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
