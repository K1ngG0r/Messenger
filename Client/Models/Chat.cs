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
        public Guid ChatId { get; set; }
        public string ChatName { get; set; } = null!;
        public string ChatImagePath { get; set; } = null!;
        public List<ChatMessage> Messages { get; set; } = null!;
        public Chat(string chatName, Guid chatId, List<ChatMessage> messages, string chatImagePath = "")
        {
            ChatId = chatId;
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
        public PrivateChat(Guid chatId, User user, string imagePath = "")
            : base(user.Name, chatId, new(), imagePath)
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
        public Participant(User user, Chat chat, ParticipantType type = ParticipantType.Member)
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
        Admin,
        Member
    }
    public class ChannelChat : Chat
    {
        public User Owner { get; set; } = null!;
        public List<Participant> Subscribers { get; set; } = new();

        public ChannelChat(Guid chatId, User owner, string channelName, string imagePath = "")
            : base(channelName, chatId, new(), imagePath)
        {
            Owner = owner;
        }
        public ChannelChat()
        {

        }
    }
    public class GroupChat : Chat
    {
        public User Owner { get; set; } = null!;
        public List<Participant> Participants { get; set; } = new();
        public GroupChat(Guid chatId, User owner, string groupName, string imagePath = "")
            : base(groupName, chatId, new(), imagePath)
        {
            Owner = owner;
        }
        public GroupChat()
        {

        }
    }
}
