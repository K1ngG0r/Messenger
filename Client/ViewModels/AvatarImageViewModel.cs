using Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Client.ViewModels
{
    public class AvatarImageViewModel:ViewModel
    {
        private string imagePath = string.Empty;
        public SolidColorBrush? BackgroundChatImage => (ImagePath == "") ? null : new SolidColorBrush(Colors.Gray);
        public string ImagePath
        {
            get => imagePath;
            set
            {
                imagePath = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BackgroundChatImage));
            }
        }
        public AvatarImageViewModel(string pathImage)
        {
            ImagePath = pathImage;
        }
        public AvatarImageViewModel()
        {
            ImagePath = string.Empty;
        }
    }
}
