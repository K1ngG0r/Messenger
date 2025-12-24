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
        public string Message { get; set; } = string.Empty;
        public DateTime When { get; set; }
        public ChatMessage()
        {

        }
        public ChatMessage(int id, User user, string message, DateTime when)
        {
            Id = id;
            Who = user;
            Message = message;
            When = when;
        }
    }
}
