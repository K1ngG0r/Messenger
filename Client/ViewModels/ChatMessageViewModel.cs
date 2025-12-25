using Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.ViewModels
{
    public class ChatMessageViewModel:ViewModel
    {
        public string Message { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public DateTime When { get; set; }
        public string State { get; set; }
        public HorizontalAlignment IsMe
        {
            get 
            {
                return HorizontalAlignment.Left;
            }
        }
        public ChatMessageViewModel(ChatMessage message)
        {
            Message = message.Message;
            Username = message.Who.Username;
            When = message.When;
            Name = message.Who.Name;
            State = message.State.ToString();
        }
    }
}
