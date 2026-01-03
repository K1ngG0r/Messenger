using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public abstract class Chat
    {
        public int Id { get; set; }
        public string ChatName { get; set; } = null!;
        public string ChatImagePath { get; set; } = null!;
        public List<ChatMessage> Messages { get; set; } = null!;
        public Chat(string chatName, List<ChatMessage> messages, string chatImagePath = "")
        {
            ChatName = chatName;
            ChatImagePath = chatImagePath;
            Messages = messages;
        }
        public Chat()
        {

        }
    }
    public class PrivateChat:Chat
    {
        public User Correspondent { get; set; } = null!;
        public PrivateChat(User user, string imagePath = "")
            : base(user.Name, new(), imagePath)
        {
            Correspondent = user;
        }
        public PrivateChat()
        {

        }
    }
    public class Participant
    {
        public int Id { get; set; }
        public User User { get; set; } = null!;
        public Chat Chat { get; set; } = null!;
        public bool IsAdmin { get; set; }
        //или права: bool canDeleteMessage и проч
        public Participant()
        {

        }
    }
    public class ChannelChat : Chat
    {
        public User Owner { get; set; } = null!;
        public List<Participant> Participants { get; set; } = new();

        public ChannelChat(string channelName, string imagePath = "")
            : base(channelName, new(), imagePath)
        {
        }
        public ChannelChat()
        {

        }
    }
    public class GroupChat : Chat
    {
        public User Owner { get; set; } = null!;
        public List<Participant> Participants { get; set; } = new();
        public GroupChat(string groupName, string imagePath = "")
            : base(groupName, new(), imagePath)
        {
        }
        public GroupChat()
        {

        }
    }
}
