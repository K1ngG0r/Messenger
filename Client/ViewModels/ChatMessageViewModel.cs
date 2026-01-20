using Client.Models;
using Client.ViewModels.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Client.ViewModels
{
    public class ChatMessageViewModel:ViewModel
    {
        private ChatService _chatService;
        private Mediator _mediator;
        public ChatMessage ChatMessage;
        public string Message => ChatMessage.Message;
        public string Name => ChatMessage.Who.Name;
        public string Username => ChatMessage.Who.Username;
        public DateTime When => ChatMessage.When;
        public string State => ChatMessage.State.ToString();
        public AvatarImageViewModel Avatar
        {
            get => new AvatarImageViewModel(ChatMessage.Who.ImagePath);
        }
        public Command DeleteCommand { get; set; }
        public Command ChatWithCommand { get; set; }
        public event Action<int> DeletionRequested = null!;
        public bool IsMe
        {
            get 
            {
                return (Username == _chatService.CurrentUser.Username);
            }
        }
        public ChatMessageViewModel(ChatMessage message, ChatService chatService, Mediator mediator)
        {
            ChatMessage = message;
            _chatService = chatService;
            DeleteCommand = new Command(OnDeleteCommand);
            ChatWithCommand = new Command(OnChatWithCommand);
            _mediator = mediator;
        }
        private void OnDeleteCommand()
        {
            DeletionRequested?.Invoke(ChatMessage.Id);
        }
        private void OnChatWithCommand()
        {
            _mediator.Send(new OpenPrivateChatMessage(ChatMessage.Who.Username));
        }
    }
}
