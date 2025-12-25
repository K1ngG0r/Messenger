using Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Patterns
{
    public class Mediator
    {
        private readonly Dictionary<Type, List<Action<object?>>> _listeners = new ();
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
            foreach(var action in actions)
                action.Invoke(message);
        }
    }
    public abstract class MediatorMessage;
    public class ChatSelectedMessage : MediatorMessage
    {
        public Chat SelectedChat { get; set; }
        public ChatSelectedMessage(Chat chat)
        {
            SelectedChat = chat;
        }
    }
    public class SendNewMessageMessage : MediatorMessage
    {
        public ChatMessage Message { get; set; }
        public SendNewMessageMessage(ChatMessage message)
        {
            Message = message;
        }
    }
}
