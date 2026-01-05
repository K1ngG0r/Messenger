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
        public ParticipantType ParticipantType { get; set; }
        public Participant(User user, Chat chat, ParticipantType type)
        {
            User = user;
            Chat = chat;
            ParticipantType = type;
        }
        public Participant()
        {

        }
    }
    public enum ParticipantType
    {
        Owner,
        Admin,
        Member
    }
    public class ChannelChat : Chat
    {
        public List<Participant> Subscribers { get; set; } = new();

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
