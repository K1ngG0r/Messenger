using Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public class ChatViewModel:ViewModel
    {
        public Chat Chat { get; set; }
        public string ChatName => Chat.ChatName;
        public string ImagePath => Chat.ChatImagePath;
        public string? LastMessage => Chat.Messages.LastOrDefault()?.Message;
        public ChatViewModel(Chat info)
        {
            Chat = info;
        }
    }
}
