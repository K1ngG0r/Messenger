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
        private CurrentUserService userService;
        public ChatMessage ChatMessage;
        public string Message => ChatMessage.Message;
        public string Name => ChatMessage.Who.Name;
        public string Username => ChatMessage.Who.Username;
        public DateTime When => ChatMessage.When;
        public string State => ChatMessage.State.ToString();
        public string ImagePath => ChatMessage.Who.ImagePath;
        public Command DeleteCommand { get; set; }
        public event Action<int> DeletionRequested = null!;
        public bool IsMe
        {
            get 
            {
                return (Username == userService.CurrentUser.Username);
            }
        }
        public ChatMessageViewModel(ChatMessage message, CurrentUserService user)
        {
            ChatMessage = message;
            userService = user;
            DeleteCommand = new Command(OnDeleteCommand);
        }
        private void OnDeleteCommand()
        {
            DeletionRequested?.Invoke(ChatMessage.Id);
        }
    }
}
