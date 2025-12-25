using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public Chat Chat { get; set; } = null!;
        public User Who { get; set; } = null!;
        public string Message { get; set; } = null!;
        public DateTime When { get; set; }
        public ChatMessageState State { get; set; }
        public ChatMessage(Chat chat, User who, string message, DateTime when, ChatMessageState state = ChatMessageState.NotDelivered)
        {
            Chat = chat;
            Who = who;
            Message = message;
            When = when;
            State = state;
        }
        public ChatMessage()
        {

        }
        public enum ChatMessageState
        {
            Received,
            Delivered,
            NotDelivered
        }
    }

    
}
