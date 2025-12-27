using Client.Models;
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
        private string _currentUser;
        public string Message { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public DateTime When { get; set; }
        public string State { get; set; }
        public int IsMe
        {
            get 
            {
                return (Username == _currentUser) ? 0 : 1;
            }
        }
        public ChatMessageViewModel(ChatMessage message, string currentUser="me")
        {
            _currentUser = currentUser;
            Message = message.Message;
            Username = message.Who.Username;
            When = message.When;
            Name = message.Who.Name;
            State = message.State.ToString();
        }
    }
}
