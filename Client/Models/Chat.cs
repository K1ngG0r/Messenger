using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public string ChatName { get; set; } = null!;
        public string ChatImagePath { get; set; } = null!;
        public List<ChatMessage> Messages { get; set; } = null!;
        public Chat(string chatName, List<ChatMessage> messages, string chatImagePath = "/cache/DefaultAvatar.png")
        {
            ChatName = chatName;
            ChatImagePath = chatImagePath;
            Messages = messages;
        }
        public Chat()
        {

        }
    }
    /*
    public abstract class Chat
    {
        public int Id { get; set; }
        public abstract string ChatName { get;}
        public List<ChatMessage> Messages { get; set; } = new();
        public Chat(int id, List<ChatMessage> messages)
        {
            Id = id;
            Messages = messages;
        }
        public Chat()
        {
        }
    }
    public class PrivateChat:Chat
    {
        public User User { get; set; } = null!;
        public override string ChatName 
        { 
            get => User.Username;
        }
        public PrivateChat()
        {

        }
    }
    public class Channel : Chat
    {
        private string channelName;
        public User Owner { get; set; } = null!;
        public List<User> Users { get; set; } = new();
        public List<User> Admins { get; set; } = new();

        public override string ChatName => channelName;

        public Channel(string name)
        {
            channelName = name;
        }
    }
    public class Group : Chat
    {
        private string groupName;
        public List<User> Users { get; set; } = new();
        public override string ChatName => groupName;
        public Group(string name)
        {
            groupName = name;
        }
    }*/
}
