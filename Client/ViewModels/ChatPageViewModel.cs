using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Client.Models;
using Client.ViewModels.Patterns;

namespace Client.ViewModels
{
    public class ChatPageViewModel : ViewModel
    {
        private ChatService _chatService;
        private CurrentUserService _userService;
        private Mediator _mediator;
        private string draftMessage = string.Empty;
        private ObservableCollection<ChatMessageViewModel> messages=new();
        private ChatInfoPageViewModel chatInfo;
        private GridLength chatColumnWidth;
        private bool isChatCreated = true;
        public Chat Chat;
        public GridLength ChatColumnWidth
        {
            get => chatColumnWidth;
            set
            {
                chatColumnWidth = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ChatMessageViewModel> Messages
        {
            get => messages;
            set
            {
                messages = value;
                OnPropertyChanged();
            }
        }
        public string ChatName => Chat.ChatName;
        public AvatarImageViewModel Avatar
        {
            get => new AvatarImageViewModel(Chat.ChatImagePath);
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
        public Command SendMessageCommand { get;}
        public Command OpenSettingsCommand { get; }
        public async Task UpdateChat(int chatId)
        {
            UpdateChat(await _chatService.LoadChatAsync(chatId));
            isChatCreated = true;
        }
        private void UpdateChat(Chat chatToUpdate)
        {
            Chat = chatToUpdate;
            Messages = new ObservableCollection<ChatMessageViewModel>(
                Chat.Messages.Select(x => RegisterChatMessageViewModel(new ChatMessageViewModel(x, _userService, _mediator))));
            ChatInfo.Update(Chat);
            OnPropertyChanged(nameof(ChatName));
            OnPropertyChanged(nameof(Avatar));
        }
        public async Task UpdateChatByUsername(string username)
        {
            var user = _chatService.TryLoadUserByUsername(username);
            if (user is null)
                return;
            var newChat = _chatService.TryLoadPrivateChatByUser(user);
            if (newChat != null)
            {
                UpdateChat(newChat);
                return;
            }
            Chat = new PrivateChat(new Guid(), user,
                AvatarsManager.GetUserAvatarPathByUsername(user.Username));
            UpdateChat(Chat);
            isChatCreated = false;
        }
        public ChatPageViewModel(string username, Mediator messenger, ChatService chatService, CurrentUserService userService)
        {
            _chatService = chatService;
            _userService = userService;
            _mediator = messenger;
            SendMessageCommand = new Command(OnSendMessage);
            _mediator.Register<ChatHistoryClearRequestedMessage>(HandleChatHistoryClearRequestedMessage);
            var user = _chatService.TryLoadUserByUsername(username);
            if (user is null)
                throw new Exception();
            var newChat = _chatService.TryLoadPrivateChatByUser(user);
            if (newChat != null)
            {
                Chat = newChat;
            }
            else
            {
                Chat = new PrivateChat(new Guid(), user,
                    AvatarsManager.GetUserAvatarPathByUsername(user.Username));
                isChatCreated = false;
            }
            messages = new ObservableCollection<ChatMessageViewModel>(
                    Chat.Messages.Select(x => RegisterChatMessageViewModel(new ChatMessageViewModel(x, _userService, _mediator))));
            chatInfo = new ChatInfoPageViewModel(Chat, _mediator, _userService);
            ChatColumnWidth = new GridLength(0);
            chatInfo.ChatInfoClosed += ChatInfo_ChatInfoClosed;
            OpenSettingsCommand = new Command(OnOpenChatSettings);
        }
        public ChatPageViewModel(int chatId, Mediator messenger, ChatService chatService, CurrentUserService userService)
        {
            _chatService = chatService;
            _userService = userService;
            _mediator = messenger;
            SendMessageCommand = new Command(OnSendMessage);
            _mediator.Register<ChatHistoryClearRequestedMessage>(HandleChatHistoryClearRequestedMessage);
            Chat = _chatService.LoadChat(chatId);
            messages = new ObservableCollection<ChatMessageViewModel>(
                Chat.Messages.Select(x => RegisterChatMessageViewModel(new ChatMessageViewModel(x, _userService, _mediator))));
            chatInfo = new ChatInfoPageViewModel(Chat, _mediator, _userService);
            ChatColumnWidth = new GridLength(0); 
            chatInfo.ChatInfoClosed += ChatInfo_ChatInfoClosed;
            OpenSettingsCommand = new Command(OnOpenChatSettings);
        }
        private async void OnSendMessage()
        {
            if (DraftMessage == string.Empty)
                return;
            if (!isChatCreated)
            {
                Chat = await _chatService.CreateNewChat(Chat);
                isChatCreated = true;
            }

            var message = await _chatService.SendMessageAsync(
                new ChatMessage(Chat, _userService.CurrentUser,
                    DraftMessage, DateTime.Now));

            Messages.Add(RegisterChatMessageViewModel(new ChatMessageViewModel(message, _userService, _mediator)));
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
        private void HandleChatHistoryClearRequestedMessage(object? newChatObject)
        {
            ChatHistoryClearRequestedMessage? message = (ChatHistoryClearRequestedMessage?)newChatObject;
            if (message is null)
                return;
            if (Chat.Id != message.ChatId)
                return;
            Messages.Clear();
            _chatService.DeleteAllMessages(Chat.Id);
        }
        private void OnOpenChatSettings()
        {
            ChatColumnWidth = new GridLength(1, GridUnitType.Star);
        }
        private void ChatInfo_ChatInfoClosed()
        {
            ChatColumnWidth = new GridLength(0, GridUnitType.Star);
        }
    }
}
