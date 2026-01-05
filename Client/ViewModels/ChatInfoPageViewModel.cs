using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Client.Models;
using Client.ViewModels.Patterns;
using Microsoft.Extensions.Primitives;

namespace Client.ViewModels
{
    public class ChatInfoPageViewModel : ViewModel
    {
        private Chat chat;
        public AvatarImageViewModel Avatar => new AvatarImageViewModel(chat?.ChatImagePath ?? string.Empty);
        public string Name => chat?.ChatName ?? string.Empty;

        public ChatInfoPageViewModel(Chat chat)
        {
            
        }
        public ChatInfoPageViewModel()
        {

        }
    }
}
