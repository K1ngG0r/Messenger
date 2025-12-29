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
        private ChatMessage chatMessage;
        public string Message => chatMessage.Message;
        public string Name => chatMessage.Who.Name;
        public string Username => chatMessage.Who.Username;
        public DateTime When => chatMessage.When;
        public string State => chatMessage.State.ToString();
        public string ImagePath => chatMessage.Who.ImagePath;
        public int IsMe
        {
            get 
            {
                return (Username == userService.CurrentUser.Username) ? 0 : 2;
            }
        }
        public ChatMessageViewModel(ChatMessage message, CurrentUserService user)
        {
            chatMessage = message;
            userService = user;
        }
    }
}
