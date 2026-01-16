using Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Patterns
{
    public interface IMediator
    {
        public void Register<MessageType>(Action<object?> action) where MessageType : MediatorMessage;
        public void Unregister<MessageType>(Action<object?> action) where MessageType : MediatorMessage;
        public void Send<MessageType>(MessageType message) where MessageType : MediatorMessage;
    }
    public class Mediator : IMediator
    {
        private readonly Dictionary<Type, List<Action<object?>>> _listeners = new();
        public void Register<MessageType>(Action<object?> action) where MessageType : MediatorMessage
        {
            Type messageType = typeof(MessageType);
            if (!_listeners.ContainsKey(messageType))
                _listeners[messageType] = new List<Action<object?>>();
            _listeners[messageType].Add(action);
        }
        public void Unregister<MessageType>(Action<object?> action) where MessageType : MediatorMessage
        {
            Type messageType = typeof(MessageType);
            if (!_listeners.ContainsKey(messageType))
                return;
            _listeners[messageType].Remove(action);
        }
        public void Send<MessageType>(MessageType message) where MessageType : MediatorMessage
        {
            Type messageType = typeof(MessageType);
            if (!_listeners.TryGetValue(messageType, out List<Action<object?>>? actions))
                return;
            var actionsCopy = actions.ToList();
            foreach (var action in actionsCopy)
                action.Invoke(message);
        }
    }
    public abstract class MediatorMessage;
    public class ChatSelectedMessage : MediatorMessage
    {
        public int ChatId { get; set; }
        public ChatSelectedMessage(int chatId)
        {
            ChatId = chatId;
        }
    }
    public class UserSelectedMessage : MediatorMessage
    {
        public string Username { get; set; }
        public UserSelectedMessage(string username)
        {
            Username = username;
        }
    }
    public class NavigateToSettingsPage : MediatorMessage
    {
        public NavigateToSettingsPage()
        {
        }
    }
    public class NavigateToMainPage : MediatorMessage
    {
        public NavigateToMainPage()
        {
        }
    }
}
