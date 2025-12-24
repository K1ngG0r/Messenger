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
        public string ChatName
        {
            get => Chat.ChatName;
        }
        //аватарка
        public ChatViewModel(Chat info)
        {
            Chat = info;
        }
    }
}
